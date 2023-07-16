using System;
using Avalonia.Controls;

namespace HXE.PlatformImpl;

public class VideoDisplay
{
    public static (uint width, uint height) GetResolution(WindowBase? window = null)
    {
        var (w, h) = (0u, 0u);

        if (window is not null && window.Screens.ScreenCount is not 0)
        {
            // I *could* use tuple assignment for style points, but it doesn't make these assignments any more succinct.
            // If-Else results in one "is not null" comparison while conditional assignment expressions would result in two comparisons.
            if (window.Screens.Primary is not null)
            {
                w = (uint)window.Screens.Primary.Bounds.Width;
                h = (uint)window.Screens.Primary.Bounds.Height;
            }
            else
            {
                Console.Warn($"Primary display could not be determined. Defaulting to first monitor in index. Identify it by Bounds '{window.Screens.All[0].Bounds}'.");
                w = (uint)window.Screens.All[0].Bounds.Width;
                h = (uint)window.Screens.All[0].Bounds.Height;
            }
        }
        else if (OperatingSystem.IsWindows())
        {
            Console.Warn("Avalonia backend is not initialized or its windowing API failed to enumerate connected displays.");
            w = (uint)Windows.PInvoke.GetSystemMetrics(Windows.PInvoke.SM_CXSCREEN);
            h = (uint)Windows.PInvoke.GetSystemMetrics(Windows.PInvoke.SM_CYSCREEN);
        }
        else if (OperatingSystem.IsLinux()) // Developed on Windows. Tested on WSL Ubuntu.
        {
            Console.Warn("Avalonia backend is not initialized or its windowing API failed to enumerate connected displays.");
            (w, h) = Linux.PInvoke.GetScreenResolutionFromXrandr();
        }
        else if (OperatingSystem.IsMacOS())
        {
            throw new NotSupportedException("This API feature does not yet support Mac OS");
        }

        if (w is 0 || h is 0)
            throw new InvalidOperationException("Unable to query the current resolution of any display/screen.");

        Console.Info($"Monitor resolution successfully acquired. ({w}x{h})");
        return (w, h);
    }
}
