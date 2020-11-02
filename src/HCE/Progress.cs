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
    public abstract SPV3.Campaign.Mission    Mission    { get; set; }
    public abstract SPV3.Campaign.Difficulty Difficulty { get; set; }

    /// <summary>
    ///   Loads object state from the inbound file.
    /// </summary>
    public void Load()
    {
      using (var reader = new BinaryReader(System.IO.File.Open(Path, FileMode.Open)))
      {
        Mission    = GetMission(ReadMission(reader));
        Difficulty = GetDifficulty(ReadDifficulty(reader));
      }
    }

    /// <summary>
    ///   Retrieves the Mission data using the inbound BinaryReader object.
    /// </summary>
    private static string ReadMission(BinaryReader reader)
    {
      const int offset = 0x1E8;
      const int length = 32;

      var data    = GetBytes(reader, offset, length);
      var mission = Encoding.UTF8.GetString(data).TrimEnd('\0');

      return mission;
    }

    /// <summary>
    ///   Retrieves the Difficulty data using the inbound BinaryReader object.
    /// </summary>
    private static byte ReadDifficulty(BinaryReader reader)
    {
      const int offset = 0x1E2;
      const int length = 1;

      var data       = GetBytes(reader, offset, length);
      var difficulty = data[0];

      return difficulty;
    }

    /// <summary>
    ///   Infers the mission and returns the Campaign.Mission representation.
    /// </summary>
    protected abstract SPV3.Campaign.Mission GetMission(string mission);

    /// <summary>
    ///   Infers the difficulty and returns the Campaign.Difficulty representation.
    /// </summary>
    protected abstract SPV3.Campaign.Difficulty GetDifficulty(byte mission);

    private static byte[] GetBytes(BinaryReader reader, int offset, int length)
    {
      var bytes = new byte[length];
      reader.BaseStream.Seek(offset, SeekOrigin.Begin);
      reader.BaseStream.Read(bytes, 0, length);
      return bytes;
    }
  }
}