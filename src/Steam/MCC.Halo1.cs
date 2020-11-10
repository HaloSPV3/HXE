using System.IO;
using static HXE.Paths.MCC;

namespace HXE.Steam
{
  partial class MCC
  {
    class Halo1
    {
      public string placeholder = "";

      public bool VerifyDefaultPath()
      {
        return System.IO.File.Exists(Halo1dll);
      }

      /// <summary>
      /// Check if the inferred Halo1.dll is probably legitimate.
      /// </summary>
      /// <returns>Returns true if the file is larger than 20MiB.</returns>
      public bool VerifyHalo1DLL()
      {
        var fileinfo = new FileInfo(Halo1Path);
        if (20971520 < (ulong)fileinfo.Length)
            return true;
        else return false;
      }
    }
  }
}
