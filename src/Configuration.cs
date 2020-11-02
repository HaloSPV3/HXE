using System.Collections.Generic;

namespace HXE
{
  /// <summary>
  ///   Configuration object.
  /// </summary>
  public class Configuration
  {
    public string                    ID       { get; set; } /* unique id used for user preference persistence         */
    public string                    Name     { get; set; } /* human-readable name of the configuration               */
    public string                    Variable { get; set; } /* representative variable/command in the initiation file */
    public List<ConfigurationOption> Options  { get; set; } /* list of available options for this configuration       */

    public class ConfigurationOption
    {
      public string Name    { get; set; } /* human readable version of this option                                   */
      public string Value   { get; set; } /* value to be assigned to the representative variable in the file         */
      public bool   Default { get; set; } /* declares this option to be the default one when the user hasn't set one */
    }
  }
}