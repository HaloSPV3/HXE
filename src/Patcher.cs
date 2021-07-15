﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using static HXE.Console;
using static HXE.Common.ExtConvert;

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
      public string Name       { get; set; } = string.Empty;   /** Make large address aware */
      public string Executable { get; set; } = string.Empty;   /** haloce.exe               */
      public bool   Toggle     { get; set; } = false;          /** Patch/Restore values     */
      public List<DataSet> DataSets = new List<DataSet>();
    }

    public class DataSet // 00000136: 0F 2F
    {
      public uint Offset   { get; set; }
      public byte Original { get; set; }
      public byte Patch    { get; set; }
    }

    /// <summary>
    /// The list of patch groups specified by pR0ps' halo ce patches.
    /// </summary>
    public static List<PatchGroup> Patches = Reader(); // See Write() for tmp overrides

    /// <summary>
    /// Reads PatchGroups from patches.crk to this instance.
    /// </summary>
    /// <returns>A list of PatchGroups read from the patches.crk file resource.</returns>
    /// <remarks>This is only used for Patches list initialization.</remarks>
    public static List<PatchGroup> Reader() 
    {
      /* Get patches.crk resource from assembly resources */
      /* Read strings from memory to...array? */
      var nl         = new string[]{ "\r\n" };
      var byteSep    = new string[]{": ", " "};
      var list       = new List<PatchGroup>();
      var patchGroup = new PatchGroup();
      var file       = Properties.Resources.patches.Split(nl, StringSplitOptions.RemoveEmptyEntries).ToList();

      file.RemoveAt(0);

      /* Remove comments from list */
      {
        var file2 = new List<string>();
        foreach (var line in file)
        {
          if (!line.StartsWith(";"))
            file2.Add(line);
        }
        file = file2;
      }

      /* Add Name, exe, and patch data to list */
      for (var index = 0; index < file.Count; index++)
      {
        /// If the first char in the line is a letter...     */
        while (index < file.Count && char.IsLetter(file.ElementAt(index).ToCharArray().First())) 
        {
          /** ...skip Name and Executable. Go to patch data. */
          index += 2;
          /** Then, if the line is patch data...             */
          /** ...assign patch name,                          */
          /** ...assign filename,                            */
          /** ...and then read patch data.                   */
          while (index < file.Count && char.IsDigit(file.ElementAt(index).ToCharArray().First()))
          {
            patchGroup = new PatchGroup() { DataSets = new List<DataSet>(),
                                            Name = file.ElementAt(index - 2),
                                            Executable = file.ElementAt(index - 1) };
            
            while (index < file.Count && char.IsDigit(file.ElementAt(index).ToCharArray().First())) 
            {
              /** Read Patch Data to List, ... 
              * Assign values{offset, original, patch} 
              * proceed to next Patch Data, 
              * then check if line is Patch Data 
              */
              List<string> values = file.
                                    ElementAt(index).Split(byteSep, StringSplitOptions.RemoveEmptyEntries).
                                    ToList();
              patchGroup.DataSets.Add(new DataSet { Offset   = ByteArrayToUInt(StringToByteArray(values[0])),
                                                    Original = StringToByteArray(values[1])[0],
                                                    Patch    = StringToByteArray(values[2])[0]});
              index++;
            }
          }
          list.Add(patchGroup);
        }
      }


      /* Parse the data to members "PatchGroup" of list "Patches" */

      /* return list */
      return list;
    }

    /// <summary>
    /// Toggle patches in Halo executable
    /// </summary>
    /// <param name="cfg">HXE.Kernel.Configuration.Tweaks.Patches</param>
    /// <param name="exePath">Path to Halo executable</param>
    public void Write(uint cfg, string exePath)
    {
      /** Configurable */
      bool DRM               = (cfg & EXEP.DISABLE_DRM_AND_KEY_CHECKS) != 0;
      bool NoGamma           = (cfg & EXEP.DISABLE_SYSTEM_GAMMA)       != 0;
      bool NoAutoCenter      = (cfg & EXEP.DISABLE_VEHICLE_AUTOCENTER) != 0;
      bool NoMouseAccel      = (cfg & EXEP.DISABLE_MOUSE_ACCELERATION) != 0;
      bool BlockCamShake     = (cfg & EXEP.BLOCK_CAMERA_SHAKE)         != 0;
      bool BlockDescopeOnDMG = (cfg & EXEP.PREVENT_DESCOPING_ON_DMG)   != 0;
      var FilteredPatches    = new List<PatchGroup>();

      /** Overrides */
      bool LAA            = true; // (cfg & EXEP.ENABLE_LARGE_ADDRESS_AWARE) != 0;
      bool FixLAN         = true; // (cfg & EXEP.BIND_SERVER_TO_0000)        != 0;
      bool Fix32Tex       = true; // (cfg & EXEP.DISABLE_SAFEMODE_PROMPT)    != 0;
      bool NoSafe         = true; // (cfg & EXEP.FIX_32BIT_TEXTURES)         != 0;
      bool NoEULA         = true; // (cfg & EXEP.DISABLE_EULA)               != 0;
      bool NoRegistryExit = true; // (cfg & EXEP.DISABLE_REG_EXIT_STATE)     != 0;
      bool BlockUpdates   = true; // (cfg & EXEP.BLOCK_UPDATE_CHECKS)        != 0;
      
      
      /** Filter PatchGroups for those requested 
       * NOTE: Update String matches as needed.
       */
      {
        var filter = new List<PatchGroup>();
        
        foreach (var pg in Patches)
        {
          if (LAA               && pg.Executable == "haloce.exe" && pg.Name.Contains("large address aware"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (DRM               && pg.Executable == "haloce.exe" && pg.Name.Contains("DRM"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (FixLAN            && pg.Executable == "haloce.exe" && pg.Name.Contains("Bind server to 0.0.0.0"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (NoSafe            && pg.Executable == "haloce.exe" && pg.Name.Contains("safe mode prompt"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (NoGamma           && pg.Executable == "haloce.exe" && pg.Name.Contains("gamma"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (Fix32Tex          && pg.Executable == "haloce.exe" && pg.Name.Contains("32-bit textures"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (NoEULA            && pg.Executable == "haloce.exe" && pg.Name.Contains("EULA"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (NoRegistryExit    && pg.Executable == "haloce.exe" && pg.Name.Contains("exit status"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (NoAutoCenter      && pg.Executable == "haloce.exe" && pg.Name.Contains("auto-centering"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (NoMouseAccel      && pg.Executable == "haloce.exe" && pg.Name.Contains("mouse acceleration"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (BlockUpdates      && pg.Executable == "haloce.exe" && pg.Name.Contains("update checks"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (BlockCamShake     && pg.Executable == "haloce.exe" && pg.Name.Contains("camera shaking"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
          if (BlockDescopeOnDMG && pg.Executable == "haloce.exe" && pg.Name.Contains("Prevent descoping when taking damage"))
          {
            pg.Toggle = true;
            filter.Add(pg);
            continue;
          }
        }

        FilteredPatches = filter;
      }

      /** Flexible patcher */
      using (var fs = new FileStream(exePath, FileMode.Open, FileAccess.ReadWrite))
      using (var ms = new MemoryStream(0x24B000))
      using (var bw = new BinaryWriter(ms))
      using (var br = new BinaryReader(ms))
      {
        foreach (var PatchGroup in FilteredPatches)
        {

          foreach (var DataSet in PatchGroup.DataSets) // I hate this
          {
            byte value  = PatchGroup.Toggle ? DataSet.Patch : DataSet.Original;
            long offset = DataSet.Offset;
            ms.Position = 0;
            fs.Position = 0;
            fs.CopyTo(ms);

            ms.Position = offset;

            if (br.ReadByte() != value)
            {
              ms.Position -= 1; /** restore position */
              bw.Write(value);  /** write value      */

              fs.Position = 0;
              ms.Position = 0;
              ms.CopyTo(fs);

              Info($"Applied \"{PatchGroup.Name}\" patch to the HCE executable");
            }
            else
            {
              Info($"HCE executable already patched with \"{PatchGroup.Name}\"");
            }
          }
        }
      }
    }

    /// <summary>
    /// Offsets for bitwise operations
    /// </summary>
    public static class EXEP
    {
      public const uint ENABLE_LARGE_ADDRESS_AWARE = 1 << 0x00; // Increase max memory range from 2GiB to 4GiB.
      public const uint DISABLE_DRM_AND_KEY_CHECKS = 1 << 0x01; // Removes several DRM/key checks.
      public const uint BIND_SERVER_TO_0000        = 1 << 0x02; // Fixes LAN game discovery. Helps systems with multiple interfaces. Can still bind to IP using `-ip` flag.
      public const uint DISABLE_SAFEMODE_PROMPT    = 1 << 0x03; // Disables prompt to restart Halo with disfunctional Safe Mode.
      public const uint DISABLE_SYSTEM_GAMMA       = 1 << 0x04; // Disables system-wide modification of display gamma. Disables in-game gamma altogether. Removes need for RegKeys.
      public const uint FIX_32BIT_TEXTURES         = 1 << 0x05; // Fixes 32-bit textures being truncated to only a blue channel.
      public const uint DISABLE_EULA               = 1 << 0x06; // Allows the game to be run without displaying the EULA. Removes the need for eula.dll to be present.
      public const uint DISABLE_REG_EXIT_STATE     = 1 << 0x07; // Makes the executable more portable by not requiring/adding registry keys.
      public const uint DISABLE_VEHICLE_AUTOCENTER = 1 << 0x08; // In stock Halo, the crosshair in vehicles (e.g. scorpion tank) will slowly move towards the horizon as you move.
      public const uint DISABLE_MOUSE_ACCELERATION = 1 << 0x09; // Self-explanatory.
      public const uint BLOCK_UPDATE_CHECKS        = 1 << 0x10; // Prevents checking for game updates.
      public const uint BLOCK_CAMERA_SHAKE         = 1 << 0x11; // Completely disable camera shake effect. Expose option to players prone to motion sickness.
      public const uint PREVENT_DESCOPING_ON_DMG   = 1 << 0x12; // Prevents zoomed-in weapons from descoping when the player takes damage.
    //public const uint ADD_TAG                    = 1 << 0x13; // pR0Ps' signature
    }
  }
}
