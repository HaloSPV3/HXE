using System.Collections.Generic;

namespace HXE
{
  /// <summary>
  ///   Resume-aware Campaign object.
  /// </summary>
  public class Campaign
  {
    public CampaignResume   Resume       { get; set; } /* awareness of campaign resuming      */
    public List<Mission>    Missions     { get; set; } /* available missions for resuming     */
    public List<Difficulty> Difficulties { get; set; } /* available difficulties for resuming */

    /// <summary>
    ///   Object representing the variable and possibility of Campaign resuming.
    /// </summary>
    public class CampaignResume
    {
      public bool   Enabled  { get; set; } /* project supports campaign resuming */
      public string Variable { get; set; } /* stores current campaign mission id */
    }
  }
}