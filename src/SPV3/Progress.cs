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
using System.Text;
using static HXE.SPV3.Campaign;

namespace HXE.SPV3
{
  /// <inheritdoc />
  /// <summary>
  ///   Object representation of a savegame.bin file on the filesystem.
  /// </summary>
  public class Progress : File
  {
    public Mission    Mission    { get; set; } = Mission.Spv3A10;
    public Difficulty Difficulty { get; set; } = Difficulty.Heroic;

    /// <summary>
    ///   Loads object state from the inbound file.
    /// </summary>
    public void Load()
    {
      using (var reader = new BinaryReader(System.IO.File.Open(Path, FileMode.Open)))
      {
        Difficulty = GetDifficulty(GetBytes(reader, 0x1E2, 1)[0]);
        Mission    = GetMission(Encoding.UTF8.GetString(GetBytes(reader, 0x1E8, 32)).TrimEnd('\0'));
      }
    }

    /// <summary>
    ///   Infers the mission and returns the Campaign.Mission representation.
    /// </summary>
    protected Mission GetMission(string mission)
    {
      switch (mission)
      {
        case "spv3a10":
          return Mission.Spv3A10;
        case "spv3a30":
          return Mission.Spv3A30;
        case "spv3a50":
          return Mission.Spv3A50;
        case "spv3b30":
          return Mission.Spv3B30;
        case "spv3b30_evolved":
          return Mission.Spv3B30Evolved;
        case "spv3b40":
          return Mission.Spv3B40;
        case "spv3c10":
          return Mission.Spv3C10;
        case "spv3c20":
          return Mission.Spv3C20;
        case "spv3c40":
          return Mission.Spv3C40;
        case "spv3d20":
          return Mission.Spv3D20;
        case "spv3d25":
          return Mission.Spv3D25;
        case "spv3d30":
          return Mission.Spv3D30;
        case "spv3d30_evolved":
          return Mission.Spv3D30Evolved;
        case "spv3d40":
          return Mission.Spv3D40;
        case "spv3_lumoria_a":
          return Mission.LumoriaA;
        case "spv3_lumoria_b":
          return Mission.LumoriaB;
        case "spv3_lumoria_cd":
          return Mission.LumoriaCd;
        case "spv3a05":
          return Mission.Spv3A05;
        default:
          return Mission.Spv3A10;
      }
    }

    /// <summary>
    ///   Infers the difficulty and returns the Campaign.Difficulty representation.
    /// </summary>
    protected Difficulty GetDifficulty(byte mission)
    {
      switch (mission)
      {
        case 0x0:
          return Difficulty.Noble;
        case 0x1:
          return Difficulty.Normal;
        case 0x2:
          return Difficulty.Heroic;
        case 0x3:
          return Difficulty.Legendary;
        default:
          return Difficulty.Normal;
      }
    }

    private static byte[] GetBytes(BinaryReader reader, int offset, int length)
    {
      var bytes = new byte[length];
      reader.BaseStream.Seek(offset, SeekOrigin.Begin);
      reader.BaseStream.Read(bytes, 0, length);
      return bytes;
    }

    /// <summary>
    ///   Represents the inbound object as a string.
    /// </summary>
    /// <param name="progress">
    ///   Object to represent as string.
    /// </param>
    /// <returns>
    ///   String representation of the inbound object.
    /// </returns>
    public static implicit operator string(Progress progress)
    {
      return progress.Path;
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
    public static explicit operator Progress(string name)
    {
      return new Progress
      {
        Path = name
      };
    }
  }
}