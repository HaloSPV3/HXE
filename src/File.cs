/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2021 Noah Sherwin
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using static System.IO.Path;

namespace HXE
{
    /// <summary>
    ///   Object defining domain rules for a file on the filesystem, and
    ///   exposing common file manipulation and management methods.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="GetTemporary()"/> if you need an instance, but do not know the <see cref="Path"/>.<br/>
    ///     A key-value dictionary of <see cref="ReaderWriterLock"/> instances is used to prevent simultaneous exclusive operations simultaneously.
    /// </remarks>
    public class File
    {
        private static readonly Dictionary<string, ReaderWriterLock> _threadingLocks = [];

        /// <summary>The path to the file.</summary>
        /// <remarks>
        /// Throws if the path is invalid e.g. null, empty, contains
        /// null character. Relative paths are resolved to the current working
        /// directory.
        /// </remarks>
        [XmlIgnore]
        public required string Path
        {
            get;
            set
            {
                field = GetFullPath(value);
                Name = GetFileName(field);
            }
        }

        public string Name { get => field ??= GetFileName(Path); set; }

        /// <summary> Creates all parent directories of <see cref="Path"/>. </summary>
        /// <exception cref="ArgumentException">Thrown by <see cref="GetDirectoryName(string?)"/></exception>
        /// <exception cref="PathTooLongException">Thrown by <see cref="GetDirectoryName(string?)"/></exception>
        ///
        /// <exception cref="ArgumentException">Thrown by <see cref="GetFullPath(string)"/></exception>
        /// <exception cref="System.Security.SecurityException">Thrown by <see cref="GetFullPath(string)"/></exception>
        /// <exception cref="ArgumentNullException">Thrown by <see cref="GetFullPath(string)"/></exception>
        /// <exception cref="NotSupportedException">Thrown by <see cref="GetFullPath(string)"/></exception>
        /// <exception cref="PathTooLongException">Thrown by <see cref="GetFullPath(string)"/></exception>
        ///
        /// <exception cref="IOException">Thrown by <see cref="Directory.CreateDirectory(string)"/></exception>
        /// <exception cref="UnauthorizedAccessException">Thrown by <see cref="Directory.CreateDirectory(string)"/></exception>
        /// <exception cref="ArgumentException">Thrown by <see cref="Directory.CreateDirectory(string)"/></exception>
        /// <exception cref="ArgumentNullException">Thrown by <see cref="Directory.CreateDirectory(string)"/></exception>
        /// <exception cref="PathTooLongException">Thrown by <see cref="Directory.CreateDirectory(string)"/></exception>
        /// <exception cref="DirectoryNotFoundException">Thrown by <see cref="Directory.CreateDirectory(string)"/></exception>
        /// <exception cref="NotSupportedException">Thrown by <see cref="Directory.CreateDirectory(string)"/></exception>
        public void CreateDirectory()
        {
            var baseDirectory = GetDirectoryName(Path) ?? /* root dir */ Path;

            if (!Directory.Exists(GetFullPath(baseDirectory)))
                Directory.CreateDirectory(baseDirectory);
        }

        public bool Exists()
        {
            return System.IO.File.Exists(Path);
        }

        /// <inheritdoc cref="System.IO.File.Exists(string)"/>
        /// <remarks>This wrapper was created because old target frameworks don't use NotNullWhen on <paramref name="path"/>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists([NotNullWhen(true)] string? path) => System.IO.File.Exists(path);

        /// <summary>
        /// Wrapper for <see cref="FileInfo.Length"/>. Measures the size of the current file in bytes.<br/>
        /// Bulk calls of this method can be slow.
        /// </summary>
        public ulong Size()
        {
            var rLock = _threadingLocks.GetOrCreateValue(Path);
            rLock.AcquireReaderLock(5_000);
            try
            {
                var file = new FileInfo(Path);
                return (ulong)file.Length;
            }
            finally
            {
                rLock.ReleaseReaderLock();
            }
        }

        public void Delete()
        {
            var wLock = _threadingLocks.GetOrCreateValue(Path);
            wLock.AcquireWriterLock(5_000);
            try
            {
                System.IO.File.Delete(Path);
            }
            finally
            {
                // Ensure that the lock is released.
                wLock.ReleaseWriterLock();
            }
        }

        public void CopyTo(string target)
        {
            var destFileName = Combine(target, Name);
            var rLockFrom = _threadingLocks.GetOrCreateValue(Path);
            var wLockTo = _threadingLocks.GetOrCreateValue(destFileName);
            rLockFrom.AcquireReaderLock(5_000);
            wLockTo.AcquireWriterLock(5_000);
            try
            {
                System.IO.File.Copy(Path, destFileName, true);
            }
            finally
            {
                rLockFrom.ReleaseReaderLock();
                wLockTo.ReleaseWriterLock();
            }
        }

        /// <remarks>Reminder: a newline character is <i>not</i> auto-appended to <paramref name="contents"/>. This allows for single-line appends.</remarks>
        public void AppendAllText(string contents)
        {
            CreateDirectory();
            var wLock = _threadingLocks.GetOrCreateValue(Path);
            wLock.AcquireWriterLock(5_000);
            try
            {
                System.IO.File.AppendAllText(Path, contents);
            }
            finally
            {
                // Ensure that the lock is released.
                wLock.ReleaseWriterLock();
            }
        }

        public void WriteAllText(string contents)
        {
            CreateDirectory();
            var wLock = _threadingLocks.GetOrCreateValue(Path);
            wLock.AcquireWriterLock(5_000);
            try
            {
                System.IO.File.WriteAllText(Path, contents + '\n');
            }
            finally
            {
                // Ensure that the lock is released.
                wLock.ReleaseWriterLock();
            }
        }

        public void WriteAllBytes(byte[] bytes)
        {
            CreateDirectory();
            var wLock = _threadingLocks.GetOrCreateValue(Path);
            wLock.AcquireWriterLock(5_000);
            try
            {
                System.IO.File.WriteAllBytes(Path, bytes);
            }
            finally
            {
                // Ensure that the lock is released.
                wLock.ReleaseWriterLock();
            }
        }

        /// <exception cref="System.ApplicationException">timeout expires before the lock request is granted.</exception>
        /// <inheritdoc cref="System.IO.File.ReadAllText(string)" path="/exception"/>
        public string ReadAllText()
        {
            var rLock = _threadingLocks.GetOrCreateValue(Path);
            rLock.AcquireReaderLock(5_000);
            try
            {
                return System.IO.File.ReadAllText(Path);
            }
            finally
            {
                rLock.ReleaseReaderLock();
            }
        }

        public byte[] ReadAllBytes()
        {
            var rLock = _threadingLocks.GetOrCreateValue(Path);
            rLock.AcquireReaderLock(5_000);
            try
            {
                return System.IO.File.ReadAllBytes(Path);
            }
            finally
            {
                rLock.ReleaseReaderLock();
            }
        }

        /// <summary> Wrapper for <see cref="System.IO.File.Move"/> </summary>
        /// <param name="destFileName"></param>
        /// <exception cref="System.ApplicationException">timeout expires before the lock request is granted.</exception>
        /// <inheritdoc cref="System.IO.File.Move(string, string)" path="/exception" />
        public void Move(string destFileName)
        {
            var rLockFrom = _threadingLocks.GetOrCreateValue(Path);
            var wLockTo = _threadingLocks.GetOrCreateValue(destFileName);
            rLockFrom.AcquireReaderLock(5_000);
            wLockTo.AcquireWriterLock(5_000);
            try
            {
                System.IO.File.Move(Path, destFileName);
            }
            finally
            {
                rLockFrom.ReleaseReaderLock();
                wLockTo.ReleaseWriterLock();
            }
        }

        /// <summary>Get a <see cref="File"/> with a temporary file path. The file is not created.</summary>
        /// <returns>A <see cref="File"/> whose path doesn't exist.</returns>
        internal static File GetTemporary() => new() { Path = Combine(GetTempPath(), GetTempFileName()) };

        /// <summary>
        ///   Represents the inbound object as a string.
        /// </summary>
        /// <param name="file">
        ///   Object to represent as string.
        /// </param>
        /// <returns>
        ///   String representation of the inbound object.
        /// </returns>
        public static implicit operator string(File file) => file.Path;

        /// <summary>
        ///   Represents the inbound string as an object.
        /// </summary>
        /// <param name="name">
        ///   String to represent as object.
        /// </param>
        /// <returns>
        ///   Object representation of the inbound string.
        /// </returns>
        public static explicit operator File(string name) => new() { Path = name };
    }

    internal static class DictionaryExtensions
    {
        [return: NotNull]
        public static T2 GetOrCreateValue<T1, T2>(this IDictionary<T1, T2> dictionary, T1 key) where T2 : new()
        {
            try
            {
                lock (dictionary)
                {
                    if (!dictionary.TryGetValue(key, out var value))
                        dictionary.Add(key, value ??= new());
                    return value!;
                }
            }
            catch (System.Exception e)
            {
                Console.Error(e.ToString());
                throw;
            }
        }
    }
}
