// Shamelessly stolen from Simon Mourier | https://stackoverflow.com/a/32888078
// LICENSE
//  Unless specified otherwise, all snippets posted to StackOverflow and related sites are licensed under
//  CC BY-SA 2.5 (before 2011-04-08),
//      3.0 (2011-04-08 to (not including) 2018-05-02),
//      or 4.0 (2018-05-02 or later)

using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace HXE.Common;

/** Quote from Simon Mourier circa 2019:
 * I have updated my answer from 2015. Here is some utility code that uses the
 * latest DPI functions from Windows 10 (specifically GetDpiForWindow function
 * which is the only method that supports the DPI_AWARENESS of the
 * window/application/process, etc.) but falls back to older ones (dpi per
 * monitor, and desktop dpi) so it should still work with Windows 7.
 *
 * It has not dependency on WPF nor Winforms, only on Windows itself.
 */

// note this class considers dpix = dpiy
[SupportedOSPlatform("windows")] // TODO: Unix equivalents
public static class DpiUtilities
{
    public struct HWND
    {
        public IntPtr Value;
        public static explicit operator HWND(IntPtr v) => new() { Value = v };
        public static implicit operator IntPtr(HWND v) => v.Value;
    }

    // you should always use this one and it will fallback if necessary
    // https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getdpiforwindow
    public static int GetDpiForWindow(HWND hwnd)
    {
        var h = LoadLibrary("user32.dll");
        var ptr = GetProcAddress(h, "GetDpiForWindow"); // Windows 10 1607
        if (ptr == IntPtr.Zero)
            return GetDpiForNearestMonitor(hwnd);

        return Marshal.GetDelegateForFunctionPointer<GetDpiForWindowFn>(ptr)(hwnd);
    }

    public static int GetDpiForNearestMonitor(HWND hwnd) => GetDpiForMonitor(GetNearestMonitorFromWindow(hwnd));
    public static int GetDpiForNearestMonitor(int x, int y) => GetDpiForMonitor(GetNearestMonitorFromPoint(x, y));
    public static int GetDpiForMonitor(IntPtr monitor, MonitorDpiType type = MonitorDpiType.Effective)
    {
        var h = LoadLibrary("shcore.dll");
        var ptr = GetProcAddress(h, "GetDpiForMonitor"); // Windows 8.1
        if (ptr == IntPtr.Zero)
            return GetDpiForDesktop();

        int hr = Marshal.GetDelegateForFunctionPointer<GetDpiForMonitorFn>(ptr)(monitor, type, out int x, out int y);
        if (hr < 0)
            return GetDpiForDesktop();

        return x;
    }

    /// <remarks>
    ///     Minimal requirements:<br/>
    ///     - Windows 7 (6.1)<br/>
    ///     OR<br/>
    ///     - Windows Vista (SP2) + Platform Update https://support.microsoft.com/en-us/topic/platform-update-supplement-for-windows-vista-and-for-windows-server-2008-5f6a1e60-0bcd-2080-06ab-85ecc8110d5f<br/>
    /// </remarks>
    [SupportedOSPlatform("windows6.0.6002")]
    public static int GetDpiForDesktop()
    {
        int hr = D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_SINGLE_THREADED, typeof(ID2D1Factory).GUID, IntPtr.Zero, out ID2D1Factory factory);
        if (hr < 0)
            return 96; // we really hit the ground, don't know what to do next!

        factory.GetDesktopDpi(out float x, out float y); // Windows 7
        Marshal.ReleaseComObject(factory);
        return (int)x;
    }

    public static IntPtr GetDesktopMonitor() => GetNearestMonitorFromWindow(GetDesktopWindow());
    public static IntPtr GetShellMonitor() => GetNearestMonitorFromWindow(GetShellWindow());
    public static IntPtr GetNearestMonitorFromWindow(HWND hwnd) => MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
    public static IntPtr GetNearestMonitorFromPoint(int x, int y) => MonitorFromPoint(new POINT { x = x, y = y }, MONITOR_DEFAULTTONEAREST);

    private delegate int GetDpiForWindowFn(HWND hwnd);
    private delegate int GetDpiForMonitorFn(IntPtr hmonitor, MonitorDpiType dpiType, out int dpiX, out int dpiY);

    private const int MONITOR_DEFAULTTONEAREST = 2;

    [SupportedOSPlatform("windows5.1")]
    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpLibFileName);

    [SupportedOSPlatform("windows5.1")]
    [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

    [SupportedOSPlatform("windows5.0")]
    [DllImport("user32")]
    private static extern IntPtr MonitorFromPoint(POINT pt, int flags);

    [SupportedOSPlatform("windows5.0")]
    [DllImport("user32")]
    private static extern IntPtr MonitorFromWindow(HWND hwnd, int flags);

    [SupportedOSPlatform("windows5.0")]
    [DllImport("user32")]
    private static extern HWND GetDesktopWindow();

    [SupportedOSPlatform("windows5.0")]
    [DllImport("user32")]
    private static extern HWND GetShellWindow();

    private partial struct POINT
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// Creates a factory object that can be used to create Direct2D resources. <see href="https://learn.microsoft.com/en-us/windows/win32/api/d2d1/nf-d2d1-d2d1createfactory"/>
    /// </summary>
    /// <remarks>
    ///     Minimal requirements:<br/>
    ///     - Windows 7 (6.1)<br/>
    ///     OR<br/>
    ///     - Windows Vista (SP2) + Platform Update https://support.microsoft.com/en-us/topic/platform-update-supplement-for-windows-vista-and-for-windows-server-2008-5f6a1e60-0bcd-2080-06ab-85ecc8110d5f<br/>
    /// </remarks>
    [SupportedOSPlatform("windows6.0.6002")]
    [DllImport("d2d1")]
    private static extern int D2D1CreateFactory(D2D1_FACTORY_TYPE factoryType, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, IntPtr pFactoryOptions, out ID2D1Factory ppIFactory);

    private enum D2D1_FACTORY_TYPE
    {
        D2D1_FACTORY_TYPE_SINGLE_THREADED = 0,
        D2D1_FACTORY_TYPE_MULTI_THREADED = 1,
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("06152247-6f50-465a-9245-118bfd3b6007")]
    private interface ID2D1Factory
    {
        int ReloadSystemMetrics();

        [PreserveSig]
        void GetDesktopDpi(out float dpiX, out float dpiY);

        // the rest is not implemented as we don't need it
    }
}

public enum MonitorDpiType
{
    Effective = 0,
    Angular = 1,
    Raw = 2,
}