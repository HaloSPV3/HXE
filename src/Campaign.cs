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

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace HXE
{
  public class Campaign : File
  {
    public Resume           Resume       { get; set; } = new Resume();
    public List<Mission>    Missions     { get; set; } = new List<Mission>();
    public List<Difficulty> Difficulties { get; set; } = new List<Difficulty>();

    public Campaign(string path)
    {
      Path = path;
    }

    public void Load()
    {
      var campaign = (Campaign) new XmlSerializer(typeof(Campaign))
        .Deserialize(new FileStream(Path, FileMode.Open));

      Resume       = campaign.Resume;
      Missions     = campaign.Missions;
      Difficulties = campaign.Difficulties;
    }
  }
}