namespace HXE
{
  /// <summary>
  ///   Campaign Mission object.
  /// </summary>
  public class Mission
  {
    public int    ID          { get; set; } /* id recognised by ui.map for campaign resuming, passed by hxe to init */
    public string Name        { get; set; } /* .map name of the mission used in cache filename and savegame.bin     */
    public string Description { get; set; } /* human-readable description/name of the map, e.g. Lumoria C & D       */
  }
}