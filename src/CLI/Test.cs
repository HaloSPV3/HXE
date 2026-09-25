using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using static System.Environment;
using static HXE.Console;

namespace HXE.CLI;

internal static class Test
{
  internal static void Run()
  {
    var test_config = new Kernel.Configuration(Path.Combine(Path.GetTempPath(), "kernel.bin"));
    using (var resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(MCC.Halo1.IdOf_343I_DER_Cer))
    {
      if (resourceStream is null) throw new NullReferenceException($"Failed to get stream for embedded resource {MCC.Halo1.IdOf_343I_DER_Cer}!");
      byte[] buffer = new byte[resourceStream.Length];
      _ = resourceStream.Read(buffer);
      Info($"certificate binary starts with: {Convert.ToHexString(buffer)[0..32]}");
      X509Certificate P7B_Fallback;
#if NET7_0_OR_GREATER
      P7B_Fallback = X509CertificateLoader.LoadCertificate(buffer);
#else
      P7B_Fallback = new X509Certificate(buffer);
#endif
    }
    Application app;
    try
    {
      Logs("Testing Settings window...");
      var test_settings = new HXE.Settings(test_config);
      app = new Application();
      // todo: do not require user input for Run to end
      _ = app.Run(test_settings);
      app.Shutdown();
      Logs("Settings Test: Succeeded");
    }
    catch (Exception e)
    {
      Error("Settings window threw an exception!" + NewLine + e.ToString());
      throw;
    }

    try
    {
      Logs("Testing Positions window...");
      var test_positions = new HXE.Positions();
      app = new Application();
      _ = app.Run(test_positions);
      app.Shutdown();
      //string target = Path.Combine(CurrentDirectory, "positions.bin");
      //Positions.Run(source, target);
      Logs("TODO: Positions test requires an OpenSauce.User.xml file.");
      Logs("Positions Test: Succeeded");
    }
    catch (Exception e)
    {
      Error("Positions window threw an exception!" + NewLine + e.ToString());
      throw;
    }
  }
}
