using System;
using System.Runtime.Versioning;

namespace HXE.Common;

internal partial class DisplayOutput
{
  [SupportedOSPlatform("Linux")]
  [SupportedOSPlatform("FreeBSD")]
  public static (
    int PrimaryScreenWidth,
    int PrimaryScreenHeight,
    float PrimaryScreenRefreshRate
    ) GetTriplet()
  {
    var xrandr = System.Diagnostics.Process.Start("xrandr", "--query")
      ?? throw new NullReferenceException("Failed to run xrandr!");
    xrandr.WaitForExit();
    if (xrandr.ExitCode != 0)
      throw new Exception($"xrandr returned exit code {xrandr.ExitCode}!\nstderr:\n${xrandr.StandardError.ReadToEnd()}");

    string stdout = xrandr.StandardOutput.ReadToEnd();
    var triplet = XRAndR_PrimaryDisplay_XYRR().Match(stdout);
    if (triplet.Groups.Count == 0) throw new Exception("xrandr did not output a resolution + refresh rate in the expected format!");
    int PrimaryScreenWidth = int.Parse(triplet.Groups[0].Value);
    int PrimaryScreenHeight = int.Parse(triplet.Groups[1].Value);
    float PrimaryScreenRefreshRate = float.Parse(triplet.Groups[2].Value);
    return (PrimaryScreenWidth, PrimaryScreenHeight, PrimaryScreenRefreshRate);
  }

#if NET7_0_OR_GREATER
  [System.Text.RegularExpressions.GeneratedRegex("\\n   (\\d+)x(\\d+) +([\\d.]+)(?=\\*+?)")]
  internal static partial System.Text.RegularExpressions.Regex XRAndR_PrimaryDisplay_XYRR();
#else
  internal static System.Text.RegularExpressions.Regex XRAndR_PrimaryDisplay_XYRR() => new("\\n   (\\d+)x(\\d+) +([\\d.]+)(?=\\*+?)");
#endif
}

