using System.IO;
using System.Text;
using HXE.SPV3;

namespace HXE.HCE
{
  /// <inheritdoc />
  /// <summary>
  ///   Abstract representation of a savegame.bin file on the filesystem.
  /// </summary>
  public abstract class Progress : File
  {
    public abstract Campaign.Mission    Mission    { get; set; }
    public abstract Campaign.Difficulty Difficulty { get; set; }

    /// <summary>
    ///   Loads object state from the inbound file.
    /// </summary>
    public void Load()
    {
      using (var reader = new BinaryReader(System.IO.File.Open(Path, FileMode.Open)))
      {
        Difficulty = GetDifficulty(GetBytes(reader, 0x1E2, 1)[0]);
        Mission    = GetMission(Encoding.UTF8.GetString(GetBytes(reader, 0x1E8, 32)).TrimEnd('\0'));
      }
    }

    /// <summary>
    ///   Infers the mission and returns the Campaign.Mission representation.
    /// </summary>
    protected abstract Campaign.Mission GetMission(string mission);

    /// <summary>
    ///   Infers the difficulty and returns the Campaign.Difficulty representation.
    /// </summary>
    protected abstract Campaign.Difficulty GetDifficulty(byte mission);

    private static byte[] GetBytes(BinaryReader reader, int offset, int length)
    {
      var bytes = new byte[length];
      reader.BaseStream.Seek(offset, SeekOrigin.Begin);
      reader.BaseStream.Read(bytes, 0, length);
      return bytes;
    }
  }
}