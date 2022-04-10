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
        // TODO: Duplicate as non-extension "CompressDirectory()"
        /// <summary>
        ///     Compress the directory represented by the DirectoryInfo object.
        /// </summary>
        /// <param name="directoryInfo">The directory to enable compression for.</param>
        /// <param name="compressFiles"></param> // TODO
        /// <param name="recurse"></param> // TODO
        /// <param name="progress"></param> // TODO
        /// <exception cref="DirectoryNotFoundException">
        /// The path encapsulated in the System.IO.DirectoryInfo object is invalid, such<br/>
        /// as being on an unmapped drive.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file is not found.</exception>
        /// <exception cref="UnauthorizedAccessException">file path is read-only.</exception>
        /// <exception cref="Win32Exception">DeviceIoControl operation failed. See <see cref="Win32Exception.NativeErrorCode"/> for exception data.</exception>
        public static void Compress(this DirectoryInfo directoryInfo, bool compressFiles = true, bool recurse = false, IProgress<Status> progress = null)
        {
            /* Progress */
            bool withProgress = progress != null;
            Status status = withProgress ? new Status
            {
                Description = $"Compressing '{directoryInfo.FullName}' and its descendents...",
                Current = 0,
                Total = 1
            } : null;

            void UpdateProgress(int n = 1)
            {
                if (!withProgress) return; // not necessary, but it's here just in case.
                status.Current += n;
                progress.Report(status);
            }

            /* Get files, subdirectories */
            SearchOption searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            DirectoryInfo[] directories = recurse ? directoryInfo.GetDirectories(
                searchPattern: "*",
                searchOption: searchOption
                ) : null;
            FileInfo[] files = compressFiles ? directoryInfo.GetFiles(
                searchPattern: "*",
                searchOption: searchOption
                ) : null;

            /* Add files, directories count to itemsTotal; Update progress */
            if (withProgress)
            {
                if (files != null)
                {
                    status.Total += files.Length;
                }
                if (directories != null)
                {
                    status.Total += directories.Length;
                }

                UpdateProgress(0);
            }

            /* Adjust current process permissions */
            System.Diagnostics.Process.GetCurrentProcess().SetSeBackupPrivilege();

            /* Compress root directory */

            SafeFileHandle directoryHandle = directoryInfo.GetHandle();

            SetCompression(directoryHandle);
            directoryHandle.Dispose();
            if (withProgress)
                UpdateProgress();

            /* Compress sub-directories */
            if (recurse)
            {
                foreach (DirectoryInfo directory in directories)
                {
                    directory.Compress();
                    if (withProgress)
                        UpdateProgress();
                }
            }

            /* Compress files*/
            if (compressFiles)
            {
                foreach (FileInfo file in files)
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
        /// <exception cref="Win32Exception">DeviceIoControl operation failed. See <see cref="Win32Exception.NativeErrorCode"/> for exception data.</exception>
        public static void Compress(this FileInfo fileInfo)
        {
            FileStream fileStream = fileInfo.Open(mode: FileMode.Open, access: FileAccess.ReadWrite, share: FileShare.None);
            SetCompression(fileStream.SafeFileHandle);
            fileStream.Dispose();
        }


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
                var error = Kernel32.GetLastError();
                switch (error)
                {
                    case Win32ErrorCode.ERROR_SHARING_VIOLATION: break;
                }
                /// TODO: Handle the following exceptions:
                /// PInvoke.Win32ErrorCode.ERROR_SHARING_VIOLATION
                ///   The process cannot access the file because it is being used by another process
                /// Win32ErrorCode.ERROR_CANT_ACCESS_FILE ???? maybe ACLs
                /// Win32ErrorCode.ERROR_FILE_CHECKED_OUT ????
                /// Win32ErrorCode.ERROR_FILE_ENCRYPTED ???? just skip the entry
                /// Win32ErrorCode.ERROR_FILE_INVALID ????
                /// Win32ErrorCode.ERROR_FILE_NOT_FOUND ???? After DirectoryInfo instance is created, the fs item may be deleted
                /// Win32ErrorCode.ERROR_FILE_OFFLINE ???? for network drives, offline files
                /// Win32ErrorCode.ERROR_FILE_READ_ONLY ???? try temporary disable
                /// Win32ErrorCode.ERROR_FILE_SYSTEM_LIMITATION ???? check for NTFS!
                /// Win32ErrorCode.ERROR_FILE_TOO_LARGE ???? Only files smaller than 30 GiB can be compressed!
                /// Win32ErrorCode.ERROR_OPEN_FILES ????
                /// Win32ErrorCode.ERROR_TOO_MANY_OPEN_FILES ???? Very rare, but possible
                /// Win32ErrorCode.ERROR_USER_MAPPED_FILE ???? part of the file is open. not sure if problem
                ThrowWin32Exception(error);
                return new SafeFileHandle(IntPtr.Zero, true);
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
            return fileStream.SafeFileHandle;
        }

        /// <summary>
        ///     P/Invoke DeviceIoControl with the FSCTL_SET_COMPRESSION
        /// </summary>
        /// <param name="handle"></param> //TODO
        /// <exception cref="Win32Exception">DeviceIoControl operation failed. See <see cref="Win32Exception.NativeErrorCode"/> for exception data.</exception>
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
                ThrowWin32Exception(Kernel32.GetLastError());
            }
        }

        internal static void ThrowWin32Exception(Win32ErrorCode errorCode, string messageAppendix = null)
        {
            throw new Win32Exception(
                error: errorCode,
                message: errorCode.GetMessage() + " : " + messageAppendix
            );
        }
    }
}
