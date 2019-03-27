/**
 * Copyright (c) 2019 Emilian Roman
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */

using System;
using System.IO;
using System.Threading.Tasks;
using static System.Console;
using static System.Environment.SpecialFolder;
using static System.Reflection.Assembly;
using static SPV3.CLI.Console;
using static SPV3.CLI.Names.Files;

namespace SPV3.CLI
{
  /// <summary>
  ///   SPV3.CLI Program.
  /// </summary>
  internal static class Program
  {
    /// <summary>
    ///   SPV3.CLI entry.
    /// </summary>
    /// <param name="args">
    ///   Arguments for the CLI.
    /// </param>
    public static void Main(string[] args)
    {
      ForegroundColor = ConsoleColor.Green;
      WriteLine(@"   _____ ____ _    _______  ________    ____");
      WriteLine(@"  / ___// __ \ |  / /__  / / ____/ /   /  _/");
      WriteLine(@"  \__ \/ /_/ / | / / /_ < / /   / /    / /  ");
      WriteLine(@" ___/ / ____/| |/ /___/ // /___/ /____/ /   ");
      WriteLine(@"/____/_/     |___//____(_)____/_____/___/   ");
      WriteLine(@"============================================");
      WriteLine(@"The SPV3.CLI Project :: GitHub: YuMiris/SPV3");
      WriteLine(@"--------------------------------------------");
      WriteLine(@"Executable has been compiled from build tag:");
      WriteLine($"{GetEntryAssembly().GetName().Version.Major}");
      WriteLine(@"--------------------------------------------");
      ForegroundColor = ConsoleColor.White;

      /**
       * Implicit Loading command.
       */
      if (args.Length == 0)
      {
        Info("Implicitly invoked 'compile' command.");

        Run(Kernel.Bootstrap);

        return;
      }

      var command = args[0];

      switch (command)
      {
        /**
         * Compilation command.
         */

        case "compile" when args.Length >= 3:
        {
          Info("Explicitly invoked 'compile' command.");

          var source = args.Length == 2 ? Environment.CurrentDirectory : args[1];
          var target = args[2];

          Run(() => { Compiler.Compile(source, target); });
          return;
        }

        /**
         * Installation command.
         */

        case "install" when args.Length >= 2:
        {
          Info("Explicitly invoked 'install' command.");

          var source = args.Length == 2 ? Environment.CurrentDirectory : args[1];
          var target = args[2];

          Run(() => { Installer.Install(source, target); });
          return;
        }

        /**
         * Placeholder command.
         */

        case "placeholder" when args.Length > 1:
        {
          Info("Explicitly invoked 'placeholder' command.");

          switch (args[1])
          {
            case "commit" when args.Length >= 4:
            {
              Info("Explicitly invoked 'commit' argument.");

              var bitmap = args[2];
              var target = args[3];
              var filter = args.Length == 4 ? "*.bitmap" : args[4];

              Run(() => { Placeholder.Commit(bitmap, target, filter); });
              return;
            }
            case "revert" when args.Length >= 2:
            {
              Info("Explicitly invoked 'revert' argument.");

              var records = args[2];

              Run(() => { Placeholder.Revert(records); });
              return;
            }
            default:
              Error("Invalid placeholder args.");
              Environment.Exit(3);
              return;
          }
        }

        /**
         * Dump command.
         */

        case "dump" when args.Length > 1:
        {
          Info("Explicitly invoked 'dump' command.");

          switch (args[1])
          {
            case "overrides":
              Info("Explicitly invoked 'overrides' argument.");

              var overridesPath = Path.Combine(Environment.GetFolderPath(MyDocuments), Overrides);

              Run(() => { new Override {Path = overridesPath}.Save(); });
              return;
            case "opensauce":
              Info("Explicitly invoked 'opensauce' argument.");

              var openSaucePath = Names.Files.OpenSauce;

              Run(() => { new OpenSauce {Path = openSaucePath}.Save(); });
              return;
            default:
              Error("Invalid dump args.");
              Environment.Exit(4);
              return;
          }
        }

        default:
          Error("Invalid command.");
          Environment.Exit(2);
          return;
      }
    }

    private static void Run(Action action)
    {
      try
      {
        Task.Run(action).GetAwaiter().GetResult();
      }
      catch (Exception e)
      {
        Error(e.Message);
        System.Console.Error.WriteLine(e.StackTrace);
      }
    }
  }
}