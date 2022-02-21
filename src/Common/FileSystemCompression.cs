/// Compress a folder using NTFS compression in .NET
/// https://stackoverflow.com/a/624446
/// - Zack Elan https://stackoverflow.com/users/2461/zack-elan
/// - Goz https://stackoverflow.com/users/131140/goz

using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace HXE.Common
{
    internal static class FileSystemCompression
    {
        private const int FSCTL_SET_COMPRESSION = 0x9C040;
        private const short COMPRESSION_FORMAT_DEFAULT = 1;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int DeviceIoControl(
            SafeFileHandle hDevice,
            int dwIoControlCode,
            ref short lpInBuffer,
            int nInBufferSize,
            IntPtr lpOutBuffer,
            int nOutBufferSize,
            ref int lpBytesReturned,
            IntPtr lpOverlapped);

        public static bool EnableCompression(SafeFileHandle handle)
        {
            int lpBytesReturned = 0;
            short lpInBuffer = COMPRESSION_FORMAT_DEFAULT;

            return DeviceIoControl(handle, FSCTL_SET_COMPRESSION,
                ref lpInBuffer, sizeof(short), IntPtr.Zero, 0,
                ref lpBytesReturned, IntPtr.Zero) != 0;
        }
    }
}
