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

        /// <summary> Creates all parent directories of <see cref="Path"/> </summary>
        /// <exception cref="System.ArgumentException">
        ///     <inheritdoc cref="GetDirectoryName(string?)" path="/exception[@cref='System.ArgumentException']"/><br/>
        ///     -OR-<br/>
        ///     <inheritdoc cref="GetFullPath(string)" path="/exception[@cref='System.ArgumentException']"/><br/>
        ///     -OR-<br/>
        ///     <inheritdoc cref="Directory.CreateDirectory(string)" path="/exception[@cref='System.ArgumentException']"/>
        /// </exception>
        /// <exception cref="PathTooLongException">
        ///     <inheritdoc cref="GetDirectoryName(string?)" path="/exception[@cref='System.IO.PathTooLongException']"/><br/>
        ///     -OR-<br/>
        ///     <inheritdoc cref="GetFullPath(string)" path="/exception[@cref='System.IO.PathTooLongException']"/><br/>
        ///     -OR-<br/>
        ///     <inheritdoc cref="Directory.CreateDirectory(string)" path="/exception[@cref='System.IO.PathTooLongException']"/>
        /// </exception>
        /// <exception cref="System.Security.SecurityException"><inheritdoc cref="GetFullPath(string)" path="/exception[@cref='System.Security.Exception']"/></exception>
        /// <exception cref="System.ArgumentNullException">
        ///     <inheritdoc cref="GetFullPath(string)" path="/exception[@cref='System.ArgumentNullException']"/><br/>
        ///     -OR-<br/>
        ///     <inheritdoc cref="Directory.CreateDirectory(string)" path="/exception[@cref='System.ArgumentNullException']"/>
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     <inheritdoc cref="GetFullPath(string)" path="/exception[@cref='System.NotSupportedException']"/><br/>
        ///     -OR-<br/>
        ///     <inheritdoc cref="Directory.CreateDirectory(string)" path="/exception[@cref='System.NotSupportedException']"/>
        /// </exception>
        /// <exception cref="IOException"><inheritdoc cref="Directory.CreateDirectory(string)" path="/exception[@cref='System.IO.IOException']"/></exception>
        /// <exception cref="System.UnauthorizedAccessException"><inheritdoc cref="Directory.CreateDirectory(string)" path="/exception[@'System.UnauthorizedAccessException']"/></exception>
        /// <exception cref="DirectoryNotFoundException"><inheritdoc cref="Directory.CreateDirectory(string)" path="exception[@cref='System.IO.DirectoryNotFoundException']"/></exception>
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
        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception" />
        /// <inheritdoc cref="ReaderWriterLock.AcquireReaderLock(int)" path="/exception" />
        /// <inheritdoc cref="FileInfo(string)" path="/exception" />
        /// <inheritdoc cref="ReaderWriterLock.ReleaseReaderLock" path="/exception" />
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

        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireWriterLock(int)" path="/exception"/>
        /// <inheritdoc cref="System.IO.File.Delete(string)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseWriterLock()" path="/exception"/>
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

        /// <summary>Copies the current file to the <paramref name="target"/> directory. Overwriting a file of the same name is not allowed. The current file must exist.</summary>
        /// <exception cref="System.ApplicationException">timeout expires before the lock request is granted.</exception>
        /// <param name="target">The name of the destination <i>directory</i>. An item with the current file's name cannot exist in the target directory.</param>
        /// <inheritdoc cref="Combine(string, string)" path="/exception"/>
        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireReaderLock(int)" path="/exception" />
        /// <inheritdoc cref="ReaderWriterLock.AcquireWriterLock(int)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseReaderLock()" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseWriterLock()" path="/exception"/>
        /// <inheritdoc cref="System.IO.File.Copy(string, string)"/>
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

        /// <inheritdoc cref="System.IO.File.AppendAllText(string, string?)"/>
        /// <remarks>Reminder: a newline character is <i>not</i> auto-appended to <paramref name="contents"/>. This allows for single-line appends.</remarks>
        /// <inheritdoc cref="CreateDirectory" path="/exception"/>
        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireWriterLock(int)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseWriterLock()" path="/exception"/>

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

        /// <exception cref="System.ApplicationException">timeout expires before the lock request is granted.</exception>
        /// <remarks>If the current file or any of its parent directories do not exist, they are created.<br/>Reminder: a newline character is _not_ auto-appended to <paramref name="contents"/>. This allows for single-line appends.</remarks>
        /// <inheritdoc cref="System.IO.File.WriteAllText(string, string?)"/>
        /// <inheritdoc cref="CreateDirectory" path="/exception"/>
        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireWriterLock(int)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseWriterLock()" path="/exception"/>
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

        /// <exception cref="System.ApplicationException">timeout expires before the lock request is granted.</exception>
        /// <remarks>If the current file or any of its parent directories do not exist, they are created.</remarks>
        /// <inheritdoc cref="System.IO.File.WriteAllBytes(string, byte[])"/>
        /// <inheritdoc cref="CreateDirectory" path="/exception"/>
        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireWriterLock(int)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseWriterLock()" path="/exception"/>
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
        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireReaderLock(int)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseReaderLock()" path="/exception"/>
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

        /// <inheritdoc cref="System.IO.File.ReadAllBytes(string)" path="/exception"/>
        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireReaderLock(int)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseReaderLock()" path="/exception"/>
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

        /// <exception cref="System.ApplicationException">timeout expires before the lock request is granted.</exception>
        /// <inheritdoc cref="System.IO.File.Move(string, string)" />
        /// <inheritdoc cref="DictionaryExtensions.GetOrCreateValue{T1, T2}(IDictionary{T1, T2}, T1)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireReaderLock(int)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.AcquireWriterLock(int)" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseReaderLock()" path="/exception"/>
        /// <inheritdoc cref="ReaderWriterLock.ReleaseWriterLock()" path="/exception"/>
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
        /// <inheritdoc cref="Combine(string, string)" path="/exception"/>
        /// <inheritdoc cref="GetTempPath" path="/exception"/>
        /// <inheritdoc cref="GetTempFileName" path="/exception"/>
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
        /// <inheritdoc cref="IDictionary{TKey, TValue}.TryGetValue(TKey, out TValue)" path="/exception" />
        /// <inheritdoc cref="IDictionary{TKey, TValue}.Add(TKey, TValue)" path="/exception" />
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
