/**
 * Copyright (c) 2022 Noah Sherwin
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
/// Compress a folder using NTFS compression in .NET
/// https://stackoverflow.com/a/624446
/// - Zack Elan https://stackoverflow.com/users/2461/zack-elan
/// - Goz https://stackoverflow.com/users/131140/goz
using System;
using System.IO;
using HXE.Extensions;
using Microsoft.Win32.SafeHandles;
using PInvoke;
using Windows.Win32.Storage.FileSystem;
using static Windows.Win32.PInvoke;

namespace HXE.Common
{
    internal static class FileSystemCompression
    {
        internal struct SubItems
        {
            public DirectoryInfo[] Directories;
            public FileInfo[] Files;
        }

        internal static SubItems GetSubItems(this DirectoryInfo rootDir, bool compressFiles, bool recurse = false) => new SubItems
        {
            Files = compressFiles ?
                rootDir.GetFiles(searchPattern: "*", searchOption: recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly) :
                Array.Empty<FileInfo>(),
            Directories = recurse ?
                rootDir.GetDirectories(searchPattern: "*", searchOption: SearchOption.AllDirectories) :
                Array.Empty<DirectoryInfo>()
        };

        // TODO: Duplicate as non-extension "CompressDirectory()"
        /// <summary>
        ///     Compress the directory represented by the DirectoryInfo object.
        /// </summary>
        /// <param name="directoryInfo">The directory to enable compression for.</param>
        /// <param name="compressFiles"></param> // TODO
        /// <param name="recurse"></param> // TODO
        /// <param name="progress"></param> // TODO
        /// <exception cref="DirectoryNotFoundException">
        ///     The path encapsulated in the System.IO.DirectoryInfo object is invalid, such<br/>
        ///     as being on an unmapped drive.
        /// </exception>
        /// <exception cref="NotSupportedException">The file system is not NTFS.</exception>
        /// <exception cref="PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">
        ///     file path is read-only.-or-<br/>
        ///     This operation is not supported on the current platform.-or-<br/>
        ///     The caller does not have the required permission.
        /// </exception>
        public static void Compress(this DirectoryInfo directoryInfo, bool compressFiles, bool recurse = false, IProgress<Status> progress = null)
        {
            ///
            /// Set up Progress
            ///
            bool withProgress = progress != null;
            Status status = withProgress ? new Status
            {
                Description = $"Compressing '{directoryInfo.FullName}' and its descendents...",
                Current = 0,
                Total = 0
            } : null;

            void UpdateProgress(int n = 1)
            {
                if (!withProgress) return; // not necessary, but it's here just in case.
                status.Current += n;
                progress.Report(status);
            }

            ///
            /// Get files, subdirectories
            ///
            SubItems? subItems = null;
            if (compressFiles || recurse)
            {
                directoryInfo.GetSubItems(compressFiles: compressFiles, recurse: recurse);
            }

            ///
            /// Add files, directories count to itemsTotal; Update progress
            ///
            if (withProgress)
            {
                if (subItems != null)
                {
                    // if (subItems != null), then (Length is always >= 0).
                    status.Total += subItems.Value.Files.Length + subItems.Value.Directories.Length;
                }

                UpdateProgress(0); // Initial update.
            }

            ///
            /// Adjust current process permissions
            ///
            System.Diagnostics.Process.GetCurrentProcess().SetSeBackupPrivilege();

            ///
            /// Compress root directory
            ///
            SafeFileHandle directoryHandle = directoryInfo.GetHandle();

            SetCompression(directoryHandle);
            directoryHandle.Dispose();
            if (withProgress)
                UpdateProgress();

            ///
            /// Compress sub-directories
            ///
            if (recurse)
            {
                foreach (DirectoryInfo directory in subItems.Value.Directories)
                {
                    directory.Compress(compressFiles: false);
                    if (withProgress)
                        UpdateProgress();
                }
            }

            ///
            ///  Compress files
            ///
            if (compressFiles)
            {
                foreach (FileInfo file in subItems.Value.Files)
                {
                    file.Compress();
                    if (withProgress)
                        UpdateProgress();
                }
            }
        }

        // TODO: Duplicate as non-extension "CompressFile()"
        /// <summary>
        ///     Set filesystem compression for the file
        /// </summary>
        /// <param name="fileInfo">A FileInfo object indicating the file to compress.</param>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">path is read-only or is a directory.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="InfoWin32Exception">DeviceIoControl operation failed. See <see cref="InfoWin32Exception.NativeErrorCode"/> for exception data.</exception>
        public static void Compress(this FileInfo fileInfo)
        {
            FileStream fileStream = fileInfo.Open(mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.None);
            SetCompression(fileStream.SafeFileHandle);
            fileStream.Dispose();
        }

        /// <summary>
        /// Get a Win32 SafeFileHandle for the specified directory
        /// </summary>
        /// <param name="directoryInfo">An existing directory the current process can access</param>
        /// <returns>A ReadWrite, NoShare SafeFileHandle representing </returns>
        /// <exception cref="InfoWin32Exception">A Win32Exception with its Message prefixed with the error code's associated string.</exception>
        public static SafeFileHandle GetHandle(this DirectoryInfo directoryInfo)
        {
            System.Diagnostics.Process.GetCurrentProcess().SetSeBackupPrivilege();

            SafeFileHandle directoryHandle = CreateFile(
                lpFileName: directoryInfo.FullName,
                dwDesiredAccess: FILE_ACCESS_FLAGS.FILE_GENERIC_READ | FILE_ACCESS_FLAGS.FILE_GENERIC_WRITE,
                dwShareMode: FILE_SHARE_MODE.FILE_SHARE_NONE,
                lpSecurityAttributes: null,
                dwCreationDisposition: FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                dwFlagsAndAttributes: FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS,
                hTemplateFile: null
                );

            if (directoryHandle.IsInvalid)
            {
                Win32ErrorCode error = Kernel32.GetLastError();
                /// TODO: Handle the following exceptions:
                /// ERROR_SHARING_VIOLATION
                ///   The process cannot access the file because it is being used by another process
                /// ERROR_ACCESS_DENIED
                ///     Access is denied.
                /// ERROR_CANT_ACCESS_FILE
                ///     The file cannot be accessed by the system. Maybe ACLs?
                /// ERROR_FILE_CHECKED_OUT
                ///     This file is checked out or locked for editing by another user.
                /// ERROR_FILE_ENCRYPTED
                ///     SKIP
                /// ERROR_FILE_INVALID ????
                /// ERROR_FILE_NOT_FOUND
                ///     After DirectoryInfo instance is created, the fs item may be deleted
                ///     SKIP
                /// ERROR_FILE_OFFLINE
                ///     for network drives, offline files
                ///     SKIP
                /// ERROR_FILE_READ_ONLY
                ///     try temporary disable
                /// ERROR_FILE_SYSTEM_LIMITATION
                ///     check for NTFS!
                /// ERROR_FILE_TOO_LARGE
                ///     The file size exceeds the limit allowed and cannot be saved.
                ///     Only files smaller than 30 GiB can be compressed!
                /// ERROR_TOO_MANY_OPEN_FILES
                ///     The system cannot open the file.
                ///     Very rare, but possible.
                ///     SKIP
                /// ERROR_USER_MAPPED_FILE
                ///     SKIP
                /// ERROR_VIRUS_INFECTED
                ///     Operation did not complete successfully because the file contains a virus or potentially unwanted software.
                ///     SKIP
                /// ERROR_VIRUS_DELETED
                ///     This file contains a virus or potentially unwanted software and cannot be opened. Due to the nature of this virus or potentially unwanted software, the file has been removed from this location.
                ///     SKIP
                switch (error)
                {
                    case Win32ErrorCode.ERROR_SHARING_VIOLATION: // The process cannot access the file because it is being used by another process
                        /// A: Wait and try again
                        /// B: Schedule the task to execute after reboot
                        /// C: Steal the file from the other process

                        // return directoryHandle;
                        break;
                    case Win32ErrorCode.ERROR_FILE_SYSTEM_LIMITATION:
                        throw new InfoWin32Exception(
                            error,
                            (
                                new DriveInfo(Path.GetPathRoot(directoryInfo.FullName)).DriveFormat != "NTFS" ?
                                "Unknown reason. " : "LZNT1 compression can be applied only on NTFS-formatted drives."
                            ) + directoryInfo.FullName
                        );
                }
                throw new InfoWin32Exception(error, directoryInfo.FullName);
            }
            else
            {
                return directoryHandle;
            }
        }

        public static SafeFileHandle GetHandle(this FileInfo fileInfo)
        {
            FileStream fileStream = fileInfo.Open(
                FileMode.Open,
                FileAccess.ReadWrite,
                FileShare.None);
            /// System.Security.SecurityException: The caller does not have the required permission.
            /// System.IO.FileNotFoundException: The file is not found.
            /// System.UnauthorizedAccessException: path is read-only or is a directory.
            /// System.IO.DirectoryNotFoundException: The specified path is invalid, such as being on an unmapped drive.
            /// System.IO.IOException: The file is already open.
            return fileStream.SafeFileHandle;
        }

        /// <summary>
        ///     P/Invoke DeviceIoControl with the FSCTL_SET_COMPRESSION
        /// </summary>
        /// <param name="handle"></param> //TODO
        /// <exception cref="InfoWin32Exception">DeviceIoControl operation failed. See <see cref="InfoWin32Exception.Message"/> for reason.</exception>
        internal static unsafe void SetCompression(SafeFileHandle handle)
        {
            uint defaultFormat = COMPRESSION_FORMAT_DEFAULT;

            if (!DeviceIoControl(
                hDevice: handle,
                dwIoControlCode: FSCTL_SET_COMPRESSION,
                lpInBuffer: &defaultFormat,
                nInBufferSize: sizeof(uint), // sizeof(typeof(lpInBuffer.GetType()))
                lpOutBuffer: (void*)IntPtr.Zero,
                nOutBufferSize: 0,
                lpBytesReturned: (uint*)IntPtr.Zero,
                lpOverlapped: (Windows.Win32.System.IO.OVERLAPPED*)IntPtr.Zero
                ))
            {
                throw new InfoWin32Exception(Kernel32.GetLastError());
            }
        }

        /// <inheritdoc cref="Windows.Win32.PInvoke.CreateFile(Windows.Win32.Foundation.PCWSTR, FILE_ACCESS_FLAGS, FILE_SHARE_MODE, Windows.Win32.Security.SECURITY_ATTRIBUTES*, FILE_CREATION_DISPOSITION, FILE_FLAGS_AND_ATTRIBUTES, Windows.Win32.Foundation.HANDLE)"/>
        /// <exception cref="InfoWin32Exception">A Win32Exception with its Message prefixed with the error code's associated string.</exception>
        internal static unsafe SafeFileHandle CreateFile(string lpFileName, FILE_ACCESS_FLAGS dwDesiredAccess, FILE_SHARE_MODE dwShareMode, Windows.Win32.Security.SECURITY_ATTRIBUTES? lpSecurityAttributes, FILE_CREATION_DISPOSITION dwCreationDisposition, FILE_FLAGS_AND_ATTRIBUTES dwFlagsAndAttributes, SafeHandleZeroOrMinusOneIsInvalid hTemplateFile)
        {
            bool hTemplateFileAddRef = false;
            try
            {
                fixed (char* lpFileNameLocal = lpFileName)
                {
                    Windows.Win32.Security.SECURITY_ATTRIBUTES lpSecurityAttributesLocal = lpSecurityAttributes ?? default;
                    Windows.Win32.Foundation.HANDLE hTemplateFileLocal;
                    if (hTemplateFile is object)
                    {
                        hTemplateFile.DangerousAddRef(ref hTemplateFileAddRef);
                        hTemplateFileLocal = (Windows.Win32.Foundation.HANDLE)hTemplateFile.DangerousGetHandle();
                    }
                    else
                    {
                        hTemplateFileLocal = default;
                    }

                    Windows.Win32.Foundation.HANDLE __result = Windows.Win32.PInvoke.CreateFile(lpFileNameLocal, dwDesiredAccess, dwShareMode, lpSecurityAttributes.HasValue ? &lpSecurityAttributesLocal : null, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFileLocal);
                    var returnHandle = new SafeFileHandle(__result, ownsHandle: true);

                    if (returnHandle.IsInvalid)
                        throw new InfoWin32Exception(error: Kernel32.GetLastError());
                    return returnHandle;
                }
            }
            finally
            {
                if (hTemplateFileAddRef)
                    hTemplateFile.DangerousRelease();
            }
        }
    }
}
