/**
 * Copyright (c) 2019 Emilian Roman
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using static System.Environment;
using static System.IO.Path;

namespace HXE
{
  /// <summary>
  ///   Lists all of the files & directories on the filesystem that SPV3 deals with.
  /// </summary>
  public static class Paths
  {
    /// <summary>
    ///   Files on the filesystem that SPV3 reads/writes.
    /// </summary>
    public static class Files
    {
      public const string Executable  = "haloce.exe";
      public const string Initiation  = "initc.txt";
      public const string Progress    = "savegame.bin";
      public const string Profile     = "blam.sav";
      public const string LastProfile = "lastprof.txt";

      public static readonly string Manifest      = $"0x{0:X8}.bin"; /* 0x0000000.bin */
      public static readonly string Installation  = Combine(Directories.HXE,       "install.txt");
      public static readonly string Configuration = Combine(Directories.HXE,       "loader.bin");
      public static readonly string Exception     = Combine(Directories.HXE,       "exception.log");
      public static readonly string OpenSauce     = Combine(Directories.OpenSauce, "OS_Settings.User.xml");
      public static readonly string Chimera       = Combine(Directories.HCE,       "chimera.bin");
    }

    /// <summary>
    ///   Directories on the filesystem that SPV3 accesses.
    /// </summary>
    public static class Directories
    {
      public const string Profiles = "savegames";

      public static readonly  string HXE       = Combine(GetFolderPath(SpecialFolder.ApplicationData), "HXE");
      private static readonly string Personal  = GetFolderPath(SpecialFolder.Personal);
      private static readonly string Games     = Combine(Personal, "My Games");
      public static readonly  string HCE       = Combine(Games,    "Halo CE");
      public static readonly  string OpenSauce = Combine(HCE,      "OpenSauce");
    }
  }
}