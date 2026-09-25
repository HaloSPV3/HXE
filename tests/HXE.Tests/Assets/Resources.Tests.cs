namespace HXE.Tests.Assets;

[TestClass]
public sealed class Resources_Should
{
    [TestMethod]
    public void ResourceNames()
    {
        Assert.AreSequenceEqual(
            expected: ["HXE.Properties.Resources.resources", "HXE.Assets.343I_DER.cer"],
            typeof(Program).Assembly.GetManifestResourceNames()
        );
    }

    [TestMethod]
    public void ResourceNameOf343I_DER()
    {
        using var resourceStream = typeof(Program).Assembly.GetManifestResourceStream("HXE.Assets.343I_DER.cer");
        Assert.IsNotNull(resourceStream);
    }
}
