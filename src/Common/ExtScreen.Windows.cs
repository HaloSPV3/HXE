#if NET462 || NET480 || (NET5_0_OR_GREATER && WINDOWS)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Windows.Win32;
using Windows.Win32.Devices.Display;
using Windows.Win32.Foundation;

namespace HXE.Common;


public static partial class ScreenExtensions
{
  #region https://stackoverflow.com/a/28257839/14894786
  [SupportedOSPlatform("windows6.0.6000")]
  private static string MonitorFriendlyName(LUID adapterId, uint targetId)
  {
    var deviceName = new DISPLAYCONFIG_TARGET_DEVICE_NAME()
    {
      header =
        {
            size = (uint)Marshal.SizeOf<DISPLAYCONFIG_TARGET_DEVICE_NAME>(),
            adapterId = adapterId,
            id = targetId,
            type = DISPLAYCONFIG_DEVICE_INFO_TYPE.DISPLAYCONFIG_DEVICE_INFO_GET_TARGET_NAME
        }
    };
    var error = (WIN32_ERROR)PInvoke.DisplayConfigGetDeviceInfo(ref deviceName.header);
    if (error != WIN32_ERROR.ERROR_SUCCESS)
      throw new Win32Exception((int)error);
    return deviceName.monitorFriendlyDeviceName.ToString();
  }

  [SupportedOSPlatform("windows6.1")]
  private static IEnumerable<string> GetAllMonitorsFriendlyNames()
  {
    uint pathCount, modeCount;
    var error = PInvoke.GetDisplayConfigBufferSizes(QUERY_DISPLAY_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS, out pathCount, out modeCount);
    if (error != WIN32_ERROR.ERROR_SUCCESS)
      throw new Win32Exception((int)error);

    var displayPaths = new DISPLAYCONFIG_PATH_INFO[pathCount];
    var displayModes = new DISPLAYCONFIG_MODE_INFO[modeCount];
    error = PInvoke.QueryDisplayConfig(QUERY_DISPLAY_CONFIG_FLAGS.QDC_ONLY_ACTIVE_PATHS,
        ref pathCount, displayPaths.AsSpan(), ref modeCount, displayModes.AsSpan());
    if (error != WIN32_ERROR.ERROR_SUCCESS)
      throw new Win32Exception((int)error);

    for (var i = 0; i < modeCount; i++)
      if (displayModes[i].infoType == DISPLAYCONFIG_MODE_INFO_TYPE.DISPLAYCONFIG_MODE_INFO_TYPE_TARGET)
        yield return MonitorFriendlyName(displayModes[i].adapterId, displayModes[i].id).ToString();
  }

  [SupportedOSPlatform("windows6.1")]
  public static string? DeviceFriendlyName(this Screen screen)
  {
    var allFriendlyNames = GetAllMonitorsFriendlyNames();
    for (var index = 0; index < Screen.AllScreens.Length; index++)
      if (Equals(screen, Screen.AllScreens[index]))
        return allFriendlyNames.ToArray()[index];
    return null;
  }
}
  #endregion
#endif
