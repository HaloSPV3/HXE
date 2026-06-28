using System;
using System.IO;
using System.Windows;
using static System.Environment;
using static HXE.Console;

namespace HXE.CLI;

internal static class Test
{
  internal static void Run()
  {
    var test_config = new Kernel.Configuration(Path.Combine(Path.GetTempPath(), "kernel.bin"));
    Application app;
    try
    {
      Logs("Testing Settings window...");
      var test_settings = new HXE.Settings(test_config);
      app = new Application();
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
