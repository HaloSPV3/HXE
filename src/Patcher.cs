using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HXE
{
  public class Patcher
  {
    /// Read patches.crk to Patches list
    /// foreach containing exe:haloce.exe, get and apply requested patches.
    /// if a patch is no longer requested (enabled), restore the original value.
    /// Patches will be passed to and stored in Kernel via bit-wise integer.
    /// Later, some patches may be configured in SPV3 loader. Perhaps via The "Advanced" menu aka HXE's Configuration UserControl. Bring it full circle.
    public class Patch
    {
      public string executable = string.Empty;
      public uint   offset     = 0x0;
      public byte   original   = 0x0;
      public byte   patch      = 0x0;
    }

    public List<Patch> Patches;


  }
}
