namespace HXE
{
  /// <summary>
  ///   Campaign Difficulty object.
  /// </summary>
  public class Difficulty
  {
    public int    ID   { get; set; } /* id representing difficulty as per savegame.bin specification  */
    public string Name { get; set; } /* human-readable name of the difficulty, e.g. Easy, Noble, etc. */
  }
}