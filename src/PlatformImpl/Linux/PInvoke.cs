using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace HXE.PlatformImpl.Linux;

/// <summary>
///
/// </summary>
/// <remarks>References<br/>
/// - https://carpenoctem.dev/blog/posix-signal-handling-in-dotnet-6/
/// - https://www.nuget.org/packages/Mono.Posix
/// </remarks>

internal static class PInvoke
{
    //[DllImport("libxrandr.so.2")]
    //public static void

    public static (uint w, uint h) GetScreenResolutionFromXrandr()
    {
        string? data = null;
        using System.Diagnostics.Process xrandr_primary = new()
        {
            StartInfo = new()
            {
                FileName = "sh",
                // should print resolution (e.g. "1920x1080") of primary display if its connected
                Arguments = "-c \"xrandr --current | grep 'connected primary' | grep --extended-regexp --only-matching '[1-9][0-9]+x[1-9][0-9]+' | head -1\"" // will match 10x10 or larger. first line of matches: 1920x1080
            }
        };

        xrandr_primary.OutputDataReceived += (sender, args) => data = args.Data;
        xrandr_primary.Start();
        xrandr_primary.BeginOutputReadLine();

        const uint timeout = 100;
        for (uint msWaited = 0; data is null && msWaited < timeout; msWaited++)
            Thread.Sleep(1);

        // could not get primary display's properties. Instead, use first monitor in list.
        if (string.IsNullOrEmpty(data))
        {
            xrandr_primary.Kill(true);
            Console.Warn(
                $"OS is '{Environment.OSVersion}'.\n" +
                $"The shell or xrandr failed to respond within {timeout}ms\n" +
                "-OR- xrandr was not found\n" +
                "-OR- xrandr did not return data\n" +
                "-OR- the Primary monitor is not connected\n" +
                "-OR- none of the connected monitors are marked 'Primary'\n" +
                "-OR- the Primary monitor's data was printed in a different format than XxY.");

            /**
            https://askubuntu.com/a/1351112
            https://superuser.com/a/603618
            */
            using System.Diagnostics.Process xrandr_first = new()
            {
                StartInfo = new()
                {
                    FileName = "sh",
                    Arguments = "-c \"xrandr --current | grep '*' | grep --extended-regexp --only-matching '[1-9][0-9]+x[1-9][0-9]+' | head -1\"",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                }
            };
            xrandr_first.OutputDataReceived += (sender, args) => data = args.Data;
            xrandr_first.Start();
            xrandr_first.BeginOutputReadLine();
            // data should be something like "1920x1080"

            for (uint msWaited = 0; data is null && msWaited < timeout; msWaited++)
                Thread.Sleep(1);

            xrandr_first.Kill(true);

            if (string.IsNullOrEmpty(data))
            {
                throw new TimeoutException($"OS is '{Environment.OSVersion}' and sh/xrandr failed to respond within 100ms or failed to get the resolution of the first connected monitor.");
            }
        }

        // If data, without Width and Height, is "x", parse data for Width and Height.
        if (data.Trim('0', '1', '2', '3', '4', '5', '6', '7', '8', '9') is "x")
        {
            string[] xy = data.Split('x', StringSplitOptions.RemoveEmptyEntries);
            Console.Info("Attempting to parse data for resolution width and height...");
            return (uint.Parse(xy[0]), uint.Parse(xy[1]));
        }
        else
        {
            throw new InvalidOperationException($"grepped string '{data}' did not match expected pattern.");
        }
    }
}
