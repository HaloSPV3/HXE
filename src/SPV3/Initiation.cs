/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2020 Noah Sherwin
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
using static HXE.Console;

namespace HXE.SPV3
{
  /// <inheritdoc />
  /// <summary>
  ///   Object representation of the OpenSauce initc.txt file on the filesystem.
  /// </summary>
  public class Initiation : File
  {
    public bool                CinemaBars        { get; set; } = false;
    public bool                PlayerAutoaim     { get; set; } = true;
    public bool                PlayerMagnetism   { get; set; } = true;
    public bool                MotionSensor      { get; set; } = true;
    public bool                MouseAcceleration { get; set; } = false;
    public int                 Gamma             { get; set; } = 0;
    public bool                Unload            { get; set; } = false;
    public Campaign.Mission    Mission           { get; set; } = Campaign.Mission.Spv3A10;
    public Campaign.Difficulty Difficulty        { get; set; } = Campaign.Difficulty.Normal;
    public bool                Unlock            { get; set; }
    public bool                Attract           { get; set; } = true;
    public int                 Shaders           { get; set; } = 0;


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
          case Campaign.Difficulty.Normal:
            return "normal";
          case Campaign.Difficulty.Heroic:
            return "hard";
          case Campaign.Difficulty.Legendary:
            return "impossible";
          case Campaign.Difficulty.Noble:
            return "easy";
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      /**
       * The SPV3 campaign maps each have an ID assigned to them. SPV3.3 breaks the compatibility by incrementing all of
       * the IDs.
       *
       * To handle permit backwards compatibility with 3.2 and below, we conditionally decrement the mission ID that
       * will be written to the initiation file.
       *
       * The decrementation is determined by the presence of the version.txt file in the working directory. If the file
       * is not found, then it's possible that this loader is being used on SPV3.2 and below, and thus we should use the
       * old (decremented) mission IDs.
       */
      int GetMission()
      {
        return System.IO.File.Exists(Paths.Legacy)
          ? (int) Mission - 1 /* compatibility with <=SPV3.2 */
          : (int) Mission;    /* compatibility with >=SPV3.3 */
      }

      var mission      = GetMission();
      var difficulty   = GetDifficulty();
      var autoaim      = PlayerAutoaim ? 1 : 0;
      var magnetism    = PlayerMagnetism ? 1 : 0;
      var cinemabars   = CinemaBars ? 0 : 1;
      var motionSensor = MotionSensor ? 1 : 0;
      var acceleration = MouseAcceleration ? 1 : 0;
      var gamma        = Gamma;

      var output = new StringBuilder();
      output.AppendLine($"set f3 {mission}");
      output.AppendLine($"set loud_dialog_hack {cinemabars}");
      output.AppendLine($"player_autoaim {autoaim}");
      output.AppendLine($"player_magnetism {magnetism}");
      output.AppendLine($"game_difficulty_set {difficulty}");
      output.AppendLine($"mouse_acceleration {acceleration}");
      output.AppendLine($"set rasterizer_hud_motion_sensor {motionSensor}");

      if (Unlock)
        output.Append("set f1 8");

      if (Attract)
        output.Append("play_bink_movie attract.bik");

      if (Gamma > 0)
        output.AppendLine($"set_gamma {gamma}");

      if (Unload)
        output.AppendLine("pp_unload");

      /**
       * Encodes post-processing settings to the initc file. Refer to doc/shaders.txt for further information.
       */

      output.AppendLine((Shaders & PP.VOLUMETRIC_LIGHTING) != 0
        ? "set rasterizer_soft_filter true"
        : "set rasterizer_soft_filter false");

      output.AppendLine((Shaders & PP.LENS_DIRT) != 0
        ? "set use_super_remote_players_action_update false"
        : "set use_super_remote_players_action_update true");

      output.AppendLine((Shaders & PP.LENS_DIRT) != 0
        ? "set use_super_remote_players_action_update false"
        : "set use_super_remote_players_action_update true");

      output.AppendLine((Shaders & PP.FILM_GRAIN) != 0
        ? "set use_new_vehicle_update_scheme false"
        : "set use_new_vehicle_update_scheme true");

      output.AppendLine((Shaders & PP.HUD_VISOR) != 0
        ? "set multiplayer_draw_teammates_names false"
        : "set multiplayer_draw_teammates_names true");

      output.AppendLine((Shaders & PP.SSR) != 0
        ? "set error_suppress_all true"
        : "set error_suppress_all false");

      if (System.IO.File.Exists(Paths.Legacy))
        output.AppendLine((Shaders & PP.DYNAMIC_LENS_FLARES) != 0
          ? "set display_precache_progress true"
          : "set display_precache_progress false");

      if (!System.IO.File.Exists(Paths.Legacy))
        output.AppendLine((Shaders & PP.DEBAND) != 0
          ? "set display_precache_progress true"
          : "set display_precache_progress false");

      /* motion blur */
      output.AppendLine("set multiplayer_hit_sound_volume " + new Func<string>(() =>
      {
        if ((Shaders & PP.MOTION_BLUR_POMB_HIGH) != 0)
          return "1.3";

        if ((Shaders & PP.MOTION_BLUR_POMB_LOW) != 0)
          return "1.2";

        if ((Shaders & PP.MOTION_BLUR_BUILT_IN) != 0)
          return "1.1";

        return "1.0";
      })());

      /* mxao */
      output.AppendLine("set cl_remote_player_action_queue_limit " + new Func<string>(() =>
      {
        if ((Shaders & PP.MXAO_HIGH) != 0)
          return "4";

        if ((Shaders & PP.MXAO_LOW) != 0)
          return "3";

        return "2";
      })());

      /* depth of field */
      output.AppendLine("set cl_remote_player_action_queue_tick_limit " + new Func<string>(() =>
      {
        if ((Shaders & PP.DOF_HIGH) != 0)
          return "8";

        if ((Shaders & PP.DOF_LOW) != 0)
          return "7";

        return "6";
      })());

      Info("Saving initiation data to the initc.txt file");
      WriteAllText(output.ToString());
      Info("Successfully applied initc.txt configurations");
      Debug("Initiation data: \n\n" + ReadAllText());
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