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

using System.IO;
using SPV3.CLI.Exceptions;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static SPV3.CLI.Names;

namespace SPV3.CLI
{
  /// <summary>
  ///   Class used for bootstrapping the SPV3 loading procedure.
  /// </summary>
  public static class Kernel
  {
    /**
     * Inic.txt can be located across multiple locationns on the filesystem; however, SPSV3 only deals with the one in
     * the working directory -- hence the name!
     */
    private static readonly Initiation RootInitc = (Initiation) Path.Combine(CurrentDirectory, Files.Initiation);

    /// <summary>
    ///   Invokes the SPV3 loading procedure.
    /// </summary>
    public static void Bootstrap()
    {
      VerifyMainAssets();
      ResumeCheckpoint();
      InvokeOverriding();
      InvokeExecutable();
    }

    /// <summary>
    ///   Invokes the SPV3.2 data verification routines.
    /// </summary>
    private static void VerifyMainAssets()
    {
      /**
       * Through the use of the manifest that was copied to the installation directory, the loader can infer the list of
       * SPV3 files on the filesystem that it must verify. Verification is done through a simple size comparison between
       * the size of the file on the filesystem and the one declared in the manifest.
       *
       * This routine relies on combining...
       *
       * - the path of the working directory; with
       * - the package's declared relative path; with
       * - the filename of the respective file
       *
       * ... to determine the absolute path of the file on the filesystem:
       * 
       * X:\Installations\SPV3\gallery\Content\editbox1024.PNG
       * |----------+---------|-------+-------|-------+-------|
       *            |                 |               |
       *            |                 |               + - File on the filesystem
       *            |                 + ----------------- Package relative path
       *            + ----------------------------------- Working directory
       */

      var manifest = (Manifest) Path.Combine(CurrentDirectory, Files.Manifest);
      
      /**
       * This shouldn't be an issue in conventional SPV3 installations; however, for existing/current SPV3 installations
       * OR installations that weren't conducted by the installer, the manifest will likely not be present. As such, we
       * have no choice but to skip the verification mechanism.
       */
      if (!manifest.Exists()) return;

      manifest.Load();

      foreach (var package in manifest.Packages)
      foreach (var entry in package.Entries)
      {
        var absolutePath = Path.Combine(CurrentDirectory, package.Path, entry.Name);
        var expectedSize = entry.Size;
        var actualSize   = new FileInfo(absolutePath).Length;

        if (expectedSize != actualSize)
          throw new AssetException($"Size mismatch {entry} (expect: {expectedSize}, actual: {actualSize}).");
      }
    }

    /// <summary>
    ///   Invokes the profile & campaign auto-detection mechanism.
    /// </summary>
    private static void ResumeCheckpoint()
    {
      /**
       * Gets the filesystem location of the directory containing HCE profiles data.
       */
      string GetRoot()
      {
        return Path.Combine(GetFolderPath(Personal), Directories.Games, Directories.Halo);
      }

      var lastprof = (LastProfile) Path.Combine(GetRoot(), Files.LastProfile);

      if (!lastprof.Exists()) return;

      lastprof.Load();

      var playerDat = (Progress) Path.Combine(GetRoot(), Directories.Profiles, lastprof.Profile, Files.Progress);

      if (!playerDat.Exists()) return;

      playerDat.Load();
      RootInitc.Mission    = playerDat.Mission;
      RootInitc.Difficulty = playerDat.Difficulty;
      RootInitc.Save();
    }

    /// <summary>
    ///   Overrides OpenSauce, Chimera & HCE/SPV3 configurations for debugging/testing purposes.
    /// </summary>
    private static void InvokeOverriding()
    {
      string GetPath()
      {
        return Path.Combine(GetFolderPath(ApplicationData), Directories.Data, Files.Overrides);
      }

      var overrides = (Override) GetPath();

      if (!overrides.Exists()) return;

      overrides.Load();
      RootInitc.PostProcessing = overrides.OpenSauce.PostProcessing;
      RootInitc.Save();
    }

    /// <summary>
    ///   Invokes the HCE executable.
    /// </summary>
    private static void InvokeExecutable()
    {
      /**
       * Gets the path of the HCE executable on the filesystem, which conventionally should be the working directory of
       * the loader, given that the loader is bundled with the rest of the SPV3.2 data.
       */
      string GetPath()
      {
        return Path.Combine(CurrentDirectory, Files.Executable);
      }

      var executable = (Executable) GetPath();

      executable.Debug.Console    = true;
      executable.Debug.Developer  = true;
      executable.Debug.Screenshot = true;

      executable.Start();
    }
  }
}