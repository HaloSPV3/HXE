using System.Collections.Generic;

namespace HXE
{
  /// <summary>
  ///   HXE Kernel Mode object.
  /// </summary>
  public class Mode
  {
    public string              ID             { get; set; } /* id of the mode for subsequent lookup */
    public string              Name           { get; set; } /* human-readable name of the mode      */
    public Campaign            Campaign       { get; set; } /* associated campaign object           */
    public List<Configuration> Configurations { get; set; } /* associated configuration object      */
  }
}