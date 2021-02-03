using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HXE
{
  public class Patcher
  {
    /// Read patches.crk to Patches list
    /// foreach containing exe:haloce.exe, get and apply requested patches.
    /// if a patch is no longer requested (enabled), restore the original value.
    /// Patches will be passed to and stored in Kernel via bit-wise integer.
    /// Later, some patches may be configured in SPV3 loader. Perhaps via The "Advanced" menu aka HXE's Configuration UserControl. Bring it full circle.
    public class PatchGroup
    {
      public string name       = string.Empty;   /* Make large address aware */
      public string executable = string.Empty;   /* haloce.exe               */
      public List<DataSet> dataSet;
    }
    public class DataSet // 00000136: 0F 2F
    {
      public uint offset   = 0x0;
      public byte original = 0x0;
      public byte patch    = 0x0;
    }

    public static List<PatchGroup> Patches = Reader();

    public static List<PatchGroup> Reader() 
    {
      /* Get patches.crk resource from assembly resources */
      System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
      string file = "patches.crk";
      file = a.GetName().Name + "." + file;

      /* Read bytes from memory to...array? */

      /* Parse the data to members "PatchGroup" of list "Patches" */

      /* return list */
      return new List<PatchGroup>();
    }

    public static class KPatches
    {
      public const uint ENABLE_LARGE_ADDRESS_AWARE = 1 << 0x00; // Increase max memory range from 2GiB to 4GiB.
      public const uint DISABLE_DRM_AND_KEY_CHECKS = 1 << 0x01; // Removes several DRM/key checks.
      public const uint BIND_SERVER_TO_0000        = 1 << 0x02; // Fixes LAN game discovery. Helps systems with multiple interfaces. Can still bind to IP using `-ip` flag.
      public const uint DISABLE_SAFEMODE_PROMPT    = 1 << 0x03; // Disables prompt to restart Halo with disfunctional Safe Mode.
      public const uint DISABLE_SYSTEM_GAMMA       = 1 << 0x04; // Disables system-wide modification of display gamma. Disables in-game gamma altogether. Removes need for RegKeys.
      public const uint DISABLE_EULA               = 1 << 0x05; // Allows the game to be run without displaying the EULA. Removes the need for eula.dll to be present.
      public const uint DISABLE_REG_EXIT_STATE     = 1 << 0x06; // Makes the executable more portable by not requiring/adding registry keys.
      public const uint DISABLE_VEHICLE_AUTOCENTER = 1 << 0x07; // In stock Halo, the crosshair in vehicles (e.g. scorpion tank) will slowly move towards the horizon as you move.
      public const uint DISABLE_MOUSE_ACCELERATION = 1 << 0x08; // Self-explanatory.
      public const uint BLOCK_UPDATE_CHECKS        = 1 << 0x09; // Prevents checking for game updates.
      public const uint PREVENT_DESCOPING_ON_DMG   = 1 << 0x10; // Prevents zoomed-in weapons from descoping when the player takes damage.
      public const uint ADD_TAG                    = 1 << 0x11; // ?
    }
  }
}
