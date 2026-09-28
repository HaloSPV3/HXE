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

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using static System.IO.Path;

namespace HXE
{
    /// <summary>
    ///   Object defining domain rules for a file on the filesystem, and
    ///   exposing common file manipulation and management methods.
    /// </summary>
    public class File
    {
        /// <summary>The path to the file.</summary>
        /// <remarks>
        /// Throws if the path is invalid e.g. null, empty, contains
        /// null character. Relative paths are resolved to the current working
        /// directory.
        /// </remarks>
        [XmlIgnore]
        public string Path
        {
            get;
            set
            {
                field = GetFullPath(value);
                Name = GetFileName(field);
            }
        }

        public string Name { get; set; }

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

        public ulong Size()
        {
            var file = new FileInfo(Path);
            return (ulong)file.Length;
        }

        public void Delete()
        {
            System.IO.File.Delete(Path);
        }

        public void CopyTo(string target)
        {
            System.IO.File.Copy(Path, Combine(target, Name), true);
        }

        public void AppendAllText(string contents)
        {
            CreateDirectory();
            System.IO.File.AppendAllText(Path, contents);
        }

        public void WriteAllText(string contents)
        {
            CreateDirectory();
            System.IO.File.WriteAllText(Path, contents);
        }

        public void WriteAllBytes(byte[] bytes)
        {
            CreateDirectory();
            System.IO.File.WriteAllBytes(Path, bytes);
        }

        public string ReadAllText()
        {
            return System.IO.File.ReadAllText(Path);
        }

        public byte[] ReadAllBytes()
        {
            return System.IO.File.ReadAllBytes(Path);
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
}
