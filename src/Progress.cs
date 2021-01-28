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
      using (var reader = new BinaryReader(System.IO.File.Open(Path, FileMode.Open)))
      {
        
        string exeDir = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        string readerString = Encoding.UTF8
                .GetString(GetBytes(reader, 0x1E8, 32))
                .TrimEnd('\0');

        Mission = readerString == "" ? /* New savegames return null string */
          campaign.Missions[0]
        : Mission = campaign.Missions
            .First(mission => mission.Value == readerString);

        Difficulty = System.IO.File.Exists(System.IO.Path.Combine(exeDir, "spv3.exe")) ?
          campaign.Difficulties[2] /* Legendary/Impossible */
        : campaign.Difficulties /* New savegames return 0 */
            .First(difficulty => difficulty.Value == GetBytes(reader, 0x1E2, 1)[0]);
      }
    }

    private static byte[] GetBytes(BinaryReader reader, int offset, int length)
    {
      var bytes = new byte[length];
      reader.BaseStream.Seek(offset, SeekOrigin.Begin);
      for (int i = 0; i < length; i++)
      {
        bytes[i] = (byte) reader.BaseStream.ReadByte();
      }
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