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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SPV3.CLI.Exceptions;
using static System.Environment;
using static System.Environment.SpecialFolder;
using static System.IO.File;
using static SPV3.CLI.Console;
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
      HeuristicInstall();
      VerifyMainAssets();
      InvokeCoreTweaks();
      ResumeCheckpoint();
      InvokeOverriding();
      InvokeExecutable();
    }

    /// <summary>
    ///   Heuristically conducts pre-loading installation, if necessary.
    /// </summary>
    private static void HeuristicInstall()
    {
      /**
       * If the HCE executable does not exist in the working directory, but the manifest and an initial package exists,
       * then we can conclude that this is an installation scenario. We can bootstrap the installer to install SPV3 to
       * the default path.
       */

      if (Exists("haloce.exe") || !Exists("0x00.bin") || !Exists("0x01.bin")) return;
      Info("Found manifest & package, but not the HCE executable. Assuming installation environment.");

      var destination = Path.Combine(GetFolderPath(Personal), Directories.Games, "Halo SPV3");
      Installer.Install(CurrentDirectory, destination);

      var cli = new ProcessStartInfo
      {
        FileName         = Path.Combine(destination, "SPV3.CLI.exe"),
        WorkingDirectory = destination
      };

      Process.Start(cli);
      Exit(0);
    }

    /// <summary>
    ///   Invokes the SPV3.2 data verification routines.
    /// </summary>
    private static void VerifyMainAssets()
    {
      /**
       * It is preferable to whitelist the type of files we would like to verify. The focus would be to skip any files
       * which are expected to be changed.
       *
       * For example, if SPV3.2 were to be distributed with a configuration file, then changing its contents would
       * result in an asset verification error. Additionally, changing the CLI executable by updating it could result in
       * the same error.
       */

      var whitelist = new List<string>
      {
        ".map",      /* map resources */
        "haloce.exe" /* game executable */
      };

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

      Info("Found manifest file - proceeding with data verification ...");

      foreach (var package in manifest.Packages)
      foreach (var entry in package.Entries)
      {
        if (!whitelist.Any(entry.Name.Contains)) /* skip verification if current file isn't in the whitelist */
          continue;

        var absolutePath = Path.Combine(CurrentDirectory, package.Path, entry.Name);
        var expectedSize = entry.Size;
        var actualSize   = new FileInfo(absolutePath).Length;

        if (expectedSize == actualSize) continue;

        Info($"Size mismatch {entry.Name} (expect: {expectedSize}, actual: {actualSize}).");
        throw new AssetException($"Size mismatch {entry.Name} (expect: {expectedSize}, actual: {actualSize}).");
      }
    }

    /// <summary>
    ///   Invokes core improvements to the auto-detected profile, such as auto max resolution and gamma fixes. This is
    ///   NOT done when a profile does not exist/cannot be found!
    /// </summary>
    private static void InvokeCoreTweaks()
    {
      var lastprof = (LastProfile) Files.LastProfile;

      if (!lastprof.Exists()) return;

      lastprof.Load();

      Info("Found lastprof file - proceeding with profile detection ...");

      var profblam = (Profile) Path.Combine(Directories.Profiles, lastprof.Profile, Files.Profile);

      if (!profblam.Exists()) return;

      profblam.Load();

      Info("Found blam.sav file - proceeding with core patches ...");

      profblam.Video.FrameRate         = Profile.ProfileVideo.VideoFrameRate.VsyncOff; /* ensure no FPS locking */
      profblam.Video.Particles         = Profile.ProfileVideo.VideoParticles.High;
      profblam.Video.Quality           = Profile.ProfileVideo.VideoQuality.High;

      profblam.Save();

      Info("Patched video resolution WIDTH  - " + profblam.Video.Resolution.Width);
      Info("Patched video resolution HEIGHT - " + profblam.Video.Resolution.Height);
      Info("Removed FPS lock! (V-Sync -> OFF)");
    }

    /// <summary>
    ///   Invokes the profile & campaign auto-detection mechanism.
    /// </summary>
    private static void ResumeCheckpoint()
    {
      var lastprof = (LastProfile) Files.LastProfile;

      if (!lastprof.Exists()) return;

      lastprof.Load();

      Info("Found lastprof file - proceeding with checkpoint detection ...");

      var playerDat = (Progress) Path.Combine(
        Directories.Profiles,
        lastprof.Profile,
        Files.Progress);

      if (!playerDat.Exists()) return;

      Info("Found checkpoint file - proceeding with resuming campaign ...");

      playerDat.Load();

      try
      {
        RootInitc.Mission    = playerDat.Mission;
        RootInitc.Difficulty = playerDat.Difficulty;
        RootInitc.Save();

        Info("Resumed campaign MISSION    - " + playerDat.Mission);
        Info("Resumed campaign DIFFICULTY - " + playerDat.Difficulty);
      }
      catch (UnauthorizedAccessException e)
      {
        Error(e.Message + " -- CAMPAIGN WILL NOT RESUME!");
      }
    }

    /// <summary>
    ///   Overrides OpenSauce, Chimera & HCE/SPV3 configurations for debugging/testing purposes.
    /// </summary>
    private static void InvokeOverriding()
    {
      string GetOverridesPath()
      {
        return Path.Combine(GetFolderPath(ApplicationData), Directories.Data, Files.Overrides);
      }

      var overrides = (Override) GetOverridesPath();
      var openSauce = (OpenSauce) Files.OpenSauce;

      if (!overrides.Exists()) return;

      Info("Found overrides file - proceeding with override preparation ...");

      /**
       * The following routine is carried out if the overrides.xml has been found in its designated directory.
       */

      overrides.Load();

      try
      {
        RootInitc.PostProcessing = overrides.OpenSauce.PostProcessing;
        RootInitc.Save();
      }
      catch (UnauthorizedAccessException e)
      {
        Error(e.Message + " -- POST-PROCESSING NOT APPLIED!");
      }

      Info("Applied post-processing effects to the initiation file.");

      if (openSauce.Exists())
        openSauce.Load();

      Info("Found OpenSauce file - proceeding with OpenSauce overriding ...");

      openSauce.Camera.FieldOfView                 = overrides.OpenSauce.Fov;
      openSauce.Camera.IgnoreFOVChangeInCinematics = overrides.OpenSauce.IgnoreCinematicsFov;

      openSauce.Rasterizer.PostProcessing.MotionBlur.Enabled =
        overrides.OpenSauce.PostProcessing.MotionBlur == PostProcessing.MotionBlurOptions.BuiltIn;

      switch (overrides.OpenSauce.PostProcessing.MotionBlur)
      {
        case PostProcessing.MotionBlurOptions.Off:
          openSauce.Rasterizer.PostProcessing.MotionBlur.BlurAmount = 0;
          break;
        case PostProcessing.MotionBlurOptions.BuiltIn:
          openSauce.Rasterizer.PostProcessing.MotionBlur.BlurAmount = 1;
          break;
        case PostProcessing.MotionBlurOptions.PombLow:
          openSauce.Rasterizer.PostProcessing.MotionBlur.BlurAmount = 2;
          break;
        case PostProcessing.MotionBlurOptions.PombHigh:
          openSauce.Rasterizer.PostProcessing.MotionBlur.BlurAmount = 3;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      openSauce.Rasterizer.PostProcessing.ExternalEffects.Enabled = overrides.OpenSauce.PostProcessing.External;
      openSauce.Rasterizer.GBuffer.Enabled                        = overrides.OpenSauce.PostProcessing.GBuffer;
      openSauce.Rasterizer.ShaderExtensions.Effect.DepthFade      = overrides.OpenSauce.PostProcessing.DepthFade;
      openSauce.Rasterizer.PostProcessing.Bloom.Enabled           = overrides.OpenSauce.PostProcessing.Bloom;

      openSauce.Save();

      Info("OpenSauce configuration has been updated with the overriding values.");
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

      if (!executable.Exists()) return;

      Info("Found HCE executable in the working directory - proceeding to execute it ...");

      executable.Debug.Console    = true;
      executable.Debug.Developer  = true;
      executable.Debug.Screenshot = true;

      Info("Debug.Console    = true");
      Info("Debug.Developer  = true");
      Info("Debug.Screenshot = true");

      Info("Using the aforementioned start-up parameters when initiating HCE process.");

      try
      {
        executable.Start();
        Info("And... we're done!");
      }
      catch (Exception e)
      {
        Error(e.Message);
      }
    }
  }
}