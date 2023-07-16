using System;
using System.Runtime.InteropServices;

namespace HXE.PlatformImpl.Windows;

internal static class PInvoke
{
    /// <summary>The width of the screen of the primary display monitor, in pixels.This is the same value obtained by calling GetDeviceCaps as follows: <c>GetDeviceCaps(hdcPrimaryMonitor, HORZRES)</c>.</summary>
    internal const int SM_CXSCREEN = 0;
    /// <summary>The height of the screen of the primary display monitor, in pixels. This is the same value obtained by calling GetDeviceCaps as follows: <c>GetDeviceCaps( hdcPrimaryMonitor, VERTRES)</c>./// </summary>
    internal const int SM_CYSCREEN = 1;

    #region user32

    [DllImport("USER32.DLL")]
    internal static extern IntPtr SetWindowPos
    (
        IntPtr hWnd,
        int hWndInsertAfter,
        int x,
        int Y,
        int cx,
        int cy,
        int wFlags
    );

    [DllImport("USER32.DLL")]
    internal static extern int SetWindowLong(
        IntPtr hWnd,
        int nIndex,
        int dwNewLong
    );

    [DllImport("USER32.DLL")]
    internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    /// <summary><see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getsystemmetrics"/></summary>
    [DllImport("USER32.DLL")]
    internal static extern int GetSystemMetrics(int nIndex);

    #endregion user32
    #region kernel32

    [DllImport("KERNEL32.DLL")]
    internal static extern IntPtr OpenProcess(
        int dwDesiredAccess,
        bool bInheritHandle,
        int dwProcessId
    );

    [DllImport("KERNEL32.DLL")]
    internal static extern bool ReadProcessMemory
    (
        int hProcess,
        int lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        ref int lpNumberOfBytesRead
    );

    #endregion
}
