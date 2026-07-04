/**
 * Copyright (c) 2021 Emilian Roman
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
using System.IO;
using System.Linq;
using System.Text;

namespace HXE
{
  public class Progress : File
  {
    public Mission    Mission    { get; set; } = new Mission();
    public Difficulty Difficulty { get; set; } = new Difficulty();

    public void Load(Campaign campaign)
    {
      if (!Exists())
        return;

      using (var reader = new BinaryReader(System.IO.File.Open(Path, FileMode.Open)))
      {
        Mission = campaign.Missions
          .FirstOrDefault
          (
            mission => mission.Value ==
              Encoding.UTF8
                .GetString(GetBytes(reader, 0x1E8, 32))
                .TrimEnd('\0')
          ) ?? campaign.Missions.First();

        Difficulty = campaign.Difficulties
          .FirstOrDefault
          (
            difficulty => difficulty.Value ==
              GetBytes(reader, 0x1E2, 1)[0]
          ) ?? campaign.Difficulties.First();
      }
    }

    private static byte[] GetBytes(BinaryReader reader, int offset, int length)
    {
#pragma warning disable CA2022 // Avoid inexact read with 'Stream.Read'
      /**
      var bytes = new byte[20]
      var offset_a = 20; // out of bounds
      var offset_b = 19; // in bounds
      */
      if (offset >= reader.BaseStream.Length)
      {
        throw new ArgumentOutOfRangeException(
          nameof(offset),
          $"Argument {nameof(offset)} is greater than the stream's length!"
        );
      }

      // if end of range is out of bounds, clip it
      ulong endOfStreamDistance = (ulong)(reader.BaseStream.Length - offset);
      ulong shortestLength = ((uint)length) < endOfStreamDistance ? (uint)length : endOfStreamDistance;
      
      byte[] bytes = new byte[shortestLength];
      reader.BaseStream.Seek(offset, SeekOrigin.Begin);
      reader.BaseStream.Read(bytes, 0, (int)shortestLength);
#pragma warning restore CA2022 // Avoid inexact read with 'Stream.Read'
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
