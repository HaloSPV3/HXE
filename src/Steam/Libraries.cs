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
    /// Read Steam's "libraryfolders.vdf" and assign each libary folder to an
    /// entry in an index array. Then, walk each entry to find the given path.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public void ParseLibraries(string path)
    {
      var file = (File) SteamLibs;
      if (!file.Exists())
        throw new Exception("Steam Library list not found.");
      using (StreamReader reader = System.IO.File.OpenText(file))
      {
        int maxlines = 255; // arbitrary limit to save memory.
        array string line[array] = { 0 };
        while ((line = reader.ReadLine()) != null)
        {
          
        }
      }
    }
  }
}
