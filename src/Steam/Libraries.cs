using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HXE.Paths.MCC;

namespace HXE.Steam
{
  class Libraries
  {
    /// <summary>
    /// Assign a file path as a string. Local functions will use the 'file' alias and read from this class member.
    /// </summary>
    public File LibrariesFile = (File)SteamLibs; // local variable 'file'
    /// <summary>
    /// Read Steam's "libraryfolders.vdf" and assign each libary folder to an
    /// entry in an index array. Then, walk each entry to find the given path.
    /// </summary>
    public void ParseLibraries()
    {
      var file = LibrariesFile;
      if (!file.Exists())
        throw new Exception("Steam Library list not found.");
      using (StreamReader reader = System.IO.File.OpenText(file))
      {
        ushort x = 0;
        ushort y = 1;
        string[] line = new string[255]; // Found more than 255 lines? That's unreasonable.
        string[] libs = new string[16]; // Arbitrary 16 library limit.
        while ((line[x] = reader.ReadLine()) != null)
        {
          if (line[x].Contains("LibraryFolders") ||
              line[x].Contains("{") ||
              line[x].Contains("TimeNextStatsReport") ||
              line[x].Contains("ContentStatsID") ||
              line[x].Contains("}"))
          {
            x++;
          }
          if (line[x].Contains($"\"{y}\""))
          {
            line[x].Trim();
            line[x].Remove(0, 3);
            line[x].Trim();
            line[x].Replace("\"", " ");
            line[x].Trim();
            libs[y - 1] = line[x]; // y-1 because 0 is ununsed. This moves all entries so the first one occupies libs[0]
            y++;
          }

          x++;
        }
      }
    }
    public void ParseLibraries()
    {

    }
  }
}

/*
"LibraryFolders"
{
	"TimeNextStatsReport"		"1605337655"
	"ContentStatsID"		"-3395695855135760621"
	"1"		"J:\\Games\\Steam"
	"2"		"I:\\MEGA\\Music\\Games\\SteamLibrary"
}
*/