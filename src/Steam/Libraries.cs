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
using System.IO;
using static HXE.Paths.MCC;

namespace HXE.Steam
{
  public class Libraries
  {
    /// <summary>
    /// Assign a file path as a string. Local functions will use the 'file' alias and read from this class member.
    /// </summary>
    public File LibrariesFile = (File)SteamLibs; // local variable 'file'
    public string[] LibList = new string[16];  // Arbitrary 16 library limit.
    /// <summary>
    /// Read Steam's "libraryfolders.vdf" and assign each libary folder to an
    /// entry in an index array. Then, walk each entry to find the given path.
    /// </summary>
    public void ParseLibraries()
    {
      if (!LibrariesFile.Exists())
        throw new Exception("Steam Library list not found.");
      using (StreamReader reader = System.IO.File.OpenText(LibrariesFile))
      {
        ushort x = 0;
        ushort y = 1;
        string[] line = new string[255]; // Found more than 255 lines? That's unreasonable.
        while ((line[x] = reader.ReadLine()) != null)
        {
          // extract the path string from the line if the line matches a pattern.
          if (line[x].Contains($"\"{y}\""))
          {
            line[x].Trim();
            line[x].Remove(0, 3);
            line[x].Trim();
            line[x].Replace("\"", " ");
            line[x].Trim();
            LibList[y - 1] = line[x]; // y-1 because 0 is ununsed. This moves all entries so the first one occupies libs[0]
            y++;
          }
          x++;
        }
      }
    }
    /// <summary>
    /// Pass a file path to LibrariesFile.vdf if the default path doesn't exist. Then execute ParseLibraries().
    /// </summary>
    /// <param name="path">The file path to non-standard LibraryFiles.vdf.</param>
    public void ParseLibraries(string path)
    {
      LibrariesFile.Path = path;
      ParseLibraries();
    }
    public void ScanLibraries(string path)
    {
      throw new NotImplementedException();
    }
  }
}