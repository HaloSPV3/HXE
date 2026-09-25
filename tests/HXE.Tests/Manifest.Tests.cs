using System.IO.Compression;

namespace HXE.Tests;

[TestClass]
public sealed class Manifest_Should
{
  [TestMethod]
  public void TestLoad()
  {
    const string xmlContent = """
<Manifest>
  <Name>manifest.bin</Name>
  <Packages>
  <Package>
      <Name>0.bin</Name>
      <Size>0</Size>
      <Entry>
        <Name>a.txt</Name>
        <Path />
        <Size>0</Size>
      </Entry>
    </Package>
    <Package>
      <Name>1.bin</Name>
      <Size>0</Size>
      <Entry>
        <Name>b.txt</Name>
        <Path />
        <Size>0</Size>
      </Entry>
    </Package>
  </Packages>
</Manifest>
""";

    var manifest = new HXE.Manifest() { Path = Path.GetTempFileName() };

    var mode = CompressionMode.Compress;
    byte[] data = System.Text.Encoding.UTF8.GetBytes(xmlContent);
    using var inflatedStream = new MemoryStream(data);
    using var deflatedStream = new MemoryStream();
    using var compressStream = new DeflateStream(deflatedStream, mode);
    inflatedStream.CopyTo(compressStream);
    compressStream.Close();
    manifest.WriteAllBytes(deflatedStream.ToArray());

    manifest.Load();
    Assert.AreEqual("manifest.bin", manifest.Name);
    Assert.HasCount(2, manifest.Packages);
    Assert.AreEquivalent(
      [
        new() {
          Entry = new() { Name = "a.txt", Path = "", Size = 0 },
          Name = "0.bin",
          Size = 0
        },
        new() {
          Entry = new() { Name = "b.txt", Path = "", Size = 0 },
          Name = "1.bin",
          Size = 0
        }
      ],
      manifest.Packages
    );
  }
}
