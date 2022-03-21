/// Compress a folder using NTFS compression in .NET
/// https://stackoverflow.com/a/624446
/// - Zack Elan https://stackoverflow.com/users/2461/zack-elan
/// - Goz https://stackoverflow.com/users/131140/goz

using System;
using System.IO;
using System.Runtime.InteropServices;
using PInvoke;

namespace HXE.Common
{
    internal static class FileSystemCompression
    {
        private static class Constants
        {
            public const int FSCTL_SET_COMPRESSION = 0x9C040;
            public const short COMPRESSION_FORMAT_DEFAULT = 1;
        }

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
            var status = withProgress ? new Status
            {
                Description = $"Compressing '{directoryInfo.FullName}' and its descendents...",
                Current = 0,
                Total = 1
            } : null;

            void UpdateProgress(int n)
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

            /* Compress root directory */
            Kernel32.SafeObjectHandle hDirectory = Kernel32.CreateFile(
                filename: directoryInfo.FullName,
                access: Kernel32.ACCESS_MASK.GenericRight.GENERIC_READ | Kernel32.ACCESS_MASK.GenericRight.GENERIC_WRITE,
                share: Kernel32.FileShare.None,
                securityAttributes: Kernel32.SECURITY_ATTRIBUTES.Create(),
                creationDisposition: Kernel32.CreationDisposition.OPEN_EXISTING,
                flagsAndAttributes: Kernel32.CreateFileFlags.FILE_FLAG_BACKUP_SEMANTICS,
                templateFile: null
            );

            SetCompression(hDirectory);
            hDirectory.Close();
            if (withProgress)
                UpdateProgress(itemsCompleted++);

            /* Compress sub-directories */
            if (recurse)
            {
                foreach (DirectoryInfo directory in directories)
                {
                    directory.Compress();
                    if (withProgress)
                        UpdateProgress(itemsCompleted++);
                }
            }

            /* Compress files*/
            if (compressFiles)
            {
                foreach (FileInfo file in files)
                {
                    file.Compress();
                    if (withProgress)
                        UpdateProgress(itemsCompleted++);
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
            SetCompression((Kernel32.SafeObjectHandle)(SafeHandle)fileStream.SafeFileHandle);
            fileStream.Dispose();
        }

        /// <summary>
        ///     P/Invoke DeviceIoControl with the FSCTL_SET_COMPRESSION
        /// </summary>
        /// <param name="handle"></param> //TODO
        /// <exception cref="Win32Exception">DeviceIoControl operation failed. See <see cref="Win32Exception.NativeErrorCode"/> for exception data.</exception>
        public static unsafe void SetCompression(Kernel32.SafeObjectHandle handle)
        {
            short lpInBuffer = Constants.COMPRESSION_FORMAT_DEFAULT;
            bool success = Kernel32.DeviceIoControl(
                hDevice: handle,
                dwIoControlCode: Constants.FSCTL_SET_COMPRESSION,
                inBuffer: &lpInBuffer,
                nInBufferSize: sizeof(short), // sizeof(typeof(lpInBuffer.GetType()))
                outBuffer: (void*)IntPtr.Zero,
                nOutBufferSize: 0,
                pBytesReturned: out _,
                lpOverlapped: (Kernel32.OVERLAPPED*)IntPtr.Zero
                );

            if (!success)
            {
                throw new Win32Exception(Kernel32.GetLastError());
            }
        }
    }
}
