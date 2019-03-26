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
using System.Text;
using static SPV3.CLI.Campaign;
using static SPV3.CLI.PostProcessing;
using static SPV3.CLI.PostProcessing.ExperimentalPostProcessing;

namespace SPV3.CLI
{
  /// <inheritdoc />
  /// <summary>
  ///   Object representation of the OpenSauce initc.txt file on the filesystem.
  /// </summary>
  public class Initiation : File
  {
    public bool           CinematicBars   { get; set; } = true;
    public bool           PlayerAutoaim   { get; set; } = true;
    public bool           PlayerMagnetism { get; set; } = true;
    public Mission        Mission         { get; set; } = Mission.Spv3A10;
    public Difficulty     Difficulty      { get; set; } = Difficulty.Normal;
    public PostProcessing PostProcessing  { get; set; } = new PostProcessing();

    /// <summary>
    ///   Saves object state to the inbound file.
    /// </summary>
    public void Save()
    {
      /**
       * Converts the Campaign.Difficulty value to a game_difficulty_set parameter, as specified in the loader.txt
       * documentation.
       */
      string GetDifficulty()
      {
        switch (Difficulty)
        {
          case Difficulty.Normal:
            return "normal";
          case Difficulty.Heroic:
            return "hard";
          case Difficulty.Legendary:
            return "impossible";
          case Difficulty.Noble:
            return "easy";
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      /**
       * Converts the PostProcessing properties' values to the f1 variable in the initc.txt file, as specified in the
       * post-processing.txt documentation.
       */
      int GetShaders()
      {
        var y = 0x0000000;

        /**
         * First, we'll handle PPE toggles.
         */

        y += !PostProcessing.DynamicLensFlares
          ? 0x0000400
          : 0x0000800;

        y += !PostProcessing.Volumetrics
          ? 0x0001000
          : 0x0002000;

        y += !PostProcessing.LensDirt
          ? 0x0004000
          : 0x0008000;

        y += !PostProcessing.HudVisor
          ? 0x0010000
          : 0x0020000;

        /**
         * Now, we'll handle value-based PPEs. 
         */

        switch (PostProcessing.Mxao)
        {
          case MxaoOptions.Off:
            y += 0x0000001;
            break;
          case MxaoOptions.Low:
            y += 0x0000002;
            break;
          case MxaoOptions.High:
            y += 0x0000004;
            break;
          default:
            y += 0x0000004;
            break;
        }

        switch (PostProcessing.Dof)
        {
          case DofOptions.Off:
            y += 0x0000008;
            break;
          case DofOptions.Low:
            y += 0x0000010;
            break;
          case DofOptions.High:
            y += 0x0000020;
            break;
          default:
            y += 0x0000020;
            break;
        }

        switch (PostProcessing.MotionBlur)
        {
          case MotionBlurOptions.Off:
            y += 0x0000040;
            break;
          case MotionBlurOptions.BuiltIn:
            y += 0x0000080;
            break;
          case MotionBlurOptions.PombLow:
            y += 0x0000100;
            break;
          case MotionBlurOptions.PombHigh:
            y += 0x0000200;
            break;
          default:
            y += 0x0000200;
            break;
        }

        switch (PostProcessing.Experimental.ThreeDimensional)
        {
          case ThreeDimensionalOptions.Off:
            y += 0x0040000;
            break;
          case ThreeDimensionalOptions.Anaglyphic:
            y += 0x0080000;
            break;
          case ThreeDimensionalOptions.Interleaving:
            y += 0x100000;
            break;
          case ThreeDimensionalOptions.SideBySide:
            y += 0x200000;
            break;
          default:
            y += 0x200000;
            break;
        }

        switch (PostProcessing.Experimental.ColorBlindMode)
        {
          case ColorBlindModeOptions.Off:
            y += 0x0400000;
            break;
          case ColorBlindModeOptions.Protanopia:
            y += 0x0800000;
            break;
          case ColorBlindModeOptions.Deuteranopes:
            y += 0x1000000;
            break;
          case ColorBlindModeOptions.Tritanopes:
            y += 0x2000000;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }

        return y;
      }

      var difficulty = GetDifficulty();
      var shaders    = GetShaders();
      var mission    = (int) Mission;
      var autoaim    = PlayerAutoaim ? 1 : 0;
      var magnetism  = PlayerMagnetism ? 1 : 0;
      var cinematic  = CinematicBars ? 1 : 0;

      var output = new StringBuilder();
      output.AppendLine($"set f1 = {shaders}");
      output.AppendLine($"set f3 = {mission}");
      output.AppendLine($"set f5 = {cinematic}");
      output.AppendLine($"player_autoaim {autoaim}");
      output.AppendLine($"player_magnetism {magnetism}");
      output.AppendLine($"game_difficulty_set {difficulty}");
      WriteAllText(output.ToString());
      
      Console.Debug("Successfully applied initc.txt configurations.");
    }

    /// <summary>
    ///   Represents the inbound object as a string.
    /// </summary>
    /// <param name="initiation">
    ///   Object to represent as string.
    /// </param>
    /// <returns>
    ///   String representation of the inbound object.
    /// </returns>
    public static implicit operator string(Initiation initiation)
    {
      return initiation.Path;
    }

    /// <summary>
    ///   Represents the inbound string as an object.
    /// </summary>
    /// <param name="name">
    ///   String to represent as object.
    /// </param>
    /// <returns>
    ///   Object representation of the inbound string.
    /// </returns>
    public static explicit operator Initiation(string name)
    {
      return new Initiation
      {
        Path = name
      };
    }
  }
}