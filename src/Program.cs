/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2023 Noah Sherwin
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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using HXE.HCE;
using static System.Console;
using static System.Environment;
using static HXE.Console;
using static HXE.Exit;
using static HXE.Properties.Resources;

namespace HXE
{
    /// <summary>
    ///   HXE Program.
    /// </summary>
    internal static class Program
    {
        internal const string Banner = @"
 _    ___   ________
| |  | \ \ / /  ____|
| |__| |\ V /| |__
|  __  | > < |  __|
| |  | |/ . \| |____
|_|  |_/_/ \_\______| :: Halo XE
=================================
A HCE wrapper and kernel for SPV3
---------------------------------
:: https://github.com/HaloSPV3/hxe
---------------------------------

HXE can be invoked with the following arguments:

      --help                 Displays commands list
      --test                 Start a dry run of HXE to self-test
      --config               Opens configuration GUI
      --positions            Opens positions GUI
      --cli                  Opens CLI instead of GUI where available
      --install=VALUE        Installs HCE/SPV3 to destination
      --compile=VALUE        Compiles HCE/SPV3 to destination
      --update=VALUE         Updates directory using manifest
      --registry=VALUE       Write to Windows Registry
      --infer                Infer the running Halo executable
      --console              Loads HCE with console mode
      --devmode              Loads HCE with developer mode
      --screenshot           Loads HCE with screenshot ability
      --window               Loads HCE in window mode
      --nogamma              Loads HCE without gamma overriding
      --adapter=VALUE        Loads HCE on monitor X
      --path=VALUE           Loads HCE with custom profile path
      --exec=VALUE           Loads HCE with custom init file
      --vidmode=VALUE        Loads HCE with video mode
      --refresh=VALUE        Loads HCE with custom refresh rate
";

        /// <summary>
        ///   HXE entry.
        /// </summary>
        /// <param name="args">
        ///   Arguments for HXE.
        /// </param>
        [STAThread]
        public static void Main(string[] args)
        {
            DisplayBanner();     /* impress our users */
            InvokeProgram(args); /* burn baby burn */
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        /// <summary>
        /// Try to start a desktop-style app with the given window as the main window. The window is shown and the app is run.
        /// </summary>
        /// <param name="args">command line arguments to pass to the Application instance.</param>
        /// <param name="window">An instance of a window, ready to be shown.</param>
        /// <returns><see langword="true"/> if successful. Otherwise, false. If an exception was thrown, it is written to the Console.StandardOut. Additional output is logged to <see cref="System.Diagnostics.Trace"/>.</returns>
        /// <remarks>Only one desktop-style app instance can be active at a time. Creating a second instance will result in an Exception.</remarks>
        public static bool TryStartWindowAsApp(string[] args, Window window)
        {
            try
            {
                var app = AppBuilder
                    .Configure<Application>()
                    .UsePlatformDetect()
                    .LogToTrace()
                    .WithInterFont()
                    .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime() { Args = args, MainWindow = window, })
                    .Instance;
                if (app is null)
                    return false;

                window.Show();
                app.Run(window);
                return true;
            }
            catch (Exception ex)
            {
                Error(ex.ToString());
                return false;
            }
        }

        /// <summary>
        ///   Console API to the HXE kernel, installer and compiler.
        /// </summary>
        /// <param name="args">
        ///   --help              Displays commands list                        <br/>
        ///   --test              Start a dry run of HXE to self-test           <br/>
        ///   --config            Opens configuration GUI                       <br/>
        ///   --positions         Opens first-person model positions GUI        <br/>
        ///   --cli               Opens CLI instead of GUI where available      <br/>
        ///   --install=VALUE     Installs HCE/SPV3   to destination            <br/>
        ///   --compile=VALUE     Compiles HCE/SPV3   to destination            <br/>
        ///   --update=VALUE      Updates directory with specified manifest     <br/>
        ///   --registry=VALUE    Write to Windows Registry                     <br/>
        ///   --infer             Infer the running Halo executable             <br/>
        ///   --console           Loads HCE           with console mode         <br/>
        ///   --devmode           Loads HCE           with developer mode       <br/>
        ///   --screenshot        Loads HCE           with screenshot ability   <br/>
        ///   --window            Loads HCE           in window mode            <br/>
        ///   --nogamma           Loads HCE           without gamma overriding  <br/>
        ///   --adapter=VALUE     Loads HCE           on monitor X              <br/>
        ///   --path=VALUE        Loads HCE           with custom profile path  <br/>
        ///   --exec=VALUE        Loads HCE           with custom init file     <br/>
        ///   --vidmode=VALUE     Loads HCE           with custom res. and Hz   <br/>
        ///   --refresh=VALUE     Loads HCE           with custom refresh rate  <br/>
        /// </param>
        /// TODO: implement --silent to run CLI without user prompts;
        /// TODO: open as GUI by default. Rework MainWindow.cs to be...useful. Lazily dump everything into it? --cli will become more useful, too.
        private static void InvokeProgram(string[] args)
        {
            Directory.CreateDirectory(Paths.Directory);

            var help = false;            /* Displays commands list              */
            var test = false;            /* Start a dry run of HXE to self-test */
            var config = false;          /* Opens configuration GUI             */
            var positions = false;       /* Opens positions GUI                 */
            var cli = false;             /* Opens CLI instead of GUI where available */
            var install = string.Empty;  /* Installs HCE/SPV3 to destination    */
            var compile = string.Empty;  /* Compiles HCE/SPV3 to destination    */
            var update = string.Empty;   /* Updates directory using manifest    */
            var registry = string.Empty; /* Write to Windows Registry           */
            var infer = false;           /* Infer the running Halo executable   */
            var console = false;         /* Loads HCE with console mode         */
            var devmode = false;         /* Loads HCE with developer mode       */
            var screenshot = false;      /* Loads HCE with screenshot ability   */
            var window = false;          /* Loads HCE in window mode            */
            var nogamma = false;         /* Loads HCE without gamma overriding  */
            var adapter = string.Empty;  /* Loads HCE on monitor X              */
            var path = string.Empty;     /* Loads HCE with custom profile path  */
            var exec = string.Empty;     /* Loads HCE with custom init file     */
            var vidmode = string.Empty;  /* Loads HCE with custom res. and Hz   */
            var refresh = string.Empty;  /* Loads HCE with custom refresh rate  */

            var options = new OptionSet()
/* hxe command   */ .Add("help", "Displays commands list", s => help = s != null)
/* hxe command   */ .Add("test", "Start a dry run of HXE to self-test", s => test = s != null)
/* hxe command   */ .Add("config", "Opens configuration GUI", s => config = s != null)
/* hxe command   */ .Add("positions", "Opens positions GUI", s => positions = s != null)
/* hxe parameter */ .Add("cli", "Enable CLI of Positions or Config", s => cli = s != null)
/* hxe parameter */ .Add("install=", "Installs HCE/SPV3 to destination", s => install = s)
/* hxe parameter */ .Add("compile=", "Compiles HCE/SPV3 to destination", s => compile = s)
/* hxe parameter */ .Add("update=", "Updates directory using manifest", s => update = s)
/* hxe parameter */ .Add("registry=", "Create Registry keys for Retail, Custom, Trial, or HEK", s => registry = s)
/* hxe parameter */ .Add("infer", "Infer the running Halo executable", s => infer = s != null)
/* hce parameter */ .Add("console", "Loads HCE with console mode", s => console = s != null)
/* hce parameter */ .Add("devmode", "Loads HCE with developer mode", s => devmode = s != null)
/* hce parameter */ .Add("screenshot", "Loads HCE with screenshot ability", s => screenshot = s != null)
/* hce parameter */ .Add("window", "Loads HCE in window mode", s => window = s != null)
/* hce parameter */ .Add("nogamma", "Loads HCE without gamma overriding", s => nogamma = s != null)
/* hce parameter */ .Add("adapter=", "Loads HCE on monitor X", s => adapter = s)
/* hce parameter */ .Add("path=", "Loads HCE with custom profile path", s => path = s)
/* hce parameter */ .Add("exec=", "Loads HCE with custom init file", s => exec = s)
/* hce parameter */ .Add("vidmode=", "Loads HCE with custom res. and Hz", s => vidmode = s)
/* hce parameter */ .Add("refresh=", "Loads HCE with custom refresh rate", s => refresh = s);

            var input = options.Parse(args); // returns *unprocessed* input

            foreach (var i in input)
                Info("Discovered CLI command: " + i);

            var hce = new Executable();
            Kernel.Configuration? configuration = null;

            if (help)
            {
                options.WriteOptionDescriptions(Out);
                WithCode(Code.Success);
            }

            if (!string.IsNullOrWhiteSpace(path))
            {
                hce.Profile.Path = path;
                configuration = new Kernel.Configuration(Paths.Custom.Configuration(path));
            }

            if (test)
            {
                // TODO: Move to Test.cs; reduce Program.cs bloat
                bool success;
                try
                {
                    Logs("Testing Settings window...");
                    Settings test_settings = new(new Kernel.Configuration(Path.Combine(Path.GetTempPath(), "kernel.bin"))) { Topmost = false, ShowActivated = true };
                    test_settings.Show();
                    success = TryStartWindowAsApp(args, test_settings) && test_settings.IsActive;
                    test_settings.Close();
                    Logs(value: "Settings Test: " + (success ? "Succeeded" : "Failed"));
                }
                catch (Exception e)
                {
                    Error("Settings window threw an exception!" + NewLine + e.ToString());
                    throw;
                }

                try
                {
                    Logs("Testing Positions window...");
                    var test_positions = new Positions() { Topmost = false, ShowActivated = true };
                    test_positions.Show();
                    success = TryStartWindowAsApp(args, test_positions) && test_positions.IsActive;
                    test_positions.Close();
                    //string target = Path.Combine(CurrentDirectory, "positions.bin");
                    //Positions.Run(source, target);
                    Logs("TODO: Positions test requires an OpenSauce.User.xml file.");
                    Logs("Positions Test: " + (success ? "Succeeded" : "Failed"));
                }
                catch (Exception e)
                {
                    Error("Positions window threw an exception!" + NewLine + e.ToString());
                    throw;
                }
                WithCode(Code.Success);
            }

            if (config)
            {
                if (TryStartWindowAsApp(args, new Settings(configuration ?? new Kernel.Configuration())))
                    WithCode(Code.Success);
                else
                    WithCode(Code.Exception);
            }

            if (positions)
            {
                if (cli)
                    CLI.Positions.Run();
                else if (!TryStartWindowAsApp(args, new Positions()))
                    WithCode(Code.Exception);

                WithCode(Code.Success);
            }

            if (infer)
            {
                var descriptions = new Dictionary<Process.Type, string>
                {
                  {Process.Type.Unknown,  "N/A"},
                  {Process.Type.Retail,   "Halo: Combat Evolved"},
                  {Process.Type.HCE,      "Halo: Custom Edition"},
                  {Process.Type.Steam,    "Halo: MCC - CEA (Steam)"},
                  {Process.Type.StoreOld, "Halo: MCC - CEA (Store)"},
                  {Process.Type.Store,    "Halo: MCC - CEA (Store)"}
                };

                Info($"Inferred the following Halo process: {descriptions[Process.Infer()]}");
                Info("Press Enter to exit");
                _ = ReadLine();
                WithCode(Code.Success);
            }

            if (!string.IsNullOrWhiteSpace(install))
            {
                Run(() =>
                {
                    SFX.Extract(new SFX.Configuration
                    {
                        Target = new DirectoryInfo(install)
                    });
                });
            }

            if (!string.IsNullOrWhiteSpace(compile))
            {
                Run(() =>
                {
                    SFX.Compile(new SFX.Configuration
                    {
                        Source = new DirectoryInfo(CurrentDirectory),
                        Target = new DirectoryInfo(compile)
                    });
                });
            }

            if (!string.IsNullOrWhiteSpace(update))
            {
                Run(() =>
                {
                    var updateModule = new Update();
                    updateModule.Import(update);
                    updateModule.Commit();
                });
            }

            if (!string.IsNullOrWhiteSpace(registry))
            {
                // TODO: Set up registry functionality
                Error("Argument 'registry' not implemented (yet!)");
                Info("Press any key to continue...");
                _ = ReadKey(intercept: true);
            }

            /**
             * Lazy search for HaloCE installations.
             */

            try
            {
                hce = Executable.Detect();
            }
            catch (Exception e)
            {
                string msg = " -- Unable to find HaloCE.exe!" + NewLine +
                             " -- Looked in working directory, Program Files, and Registry." + NewLine +
                             " -- The working directory is " + CurrentDirectory + NewLine +
                             " -- Error:  " + NewLine +
                             e.ToString();
                var log = (File)Paths.Exception;
                log.AppendAllText(msg + NewLine);
                Error(msg);
            }

            if (console)
                hce.Debug.Console = true;

            if (devmode)
                hce.Debug.Developer = true;

            if (screenshot)
                hce.Debug.Screenshot = true;

            if (window)
                hce.Video.Window = true;

            if (nogamma)
                hce.Video.NoGamma = true;

            if (!string.IsNullOrWhiteSpace(adapter))
                hce.Video.Adapter = byte.Parse(adapter);

            if (!string.IsNullOrWhiteSpace(exec))
                hce.Debug.Initiation = exec;

            if (!string.IsNullOrWhiteSpace(vidmode))
            {
                var a = vidmode.Split(',');

                if (a.Length < 2) return;

                hce.Video.DisplayMode = true;
                hce.Video.Width = ushort.Parse(a[0]);
                hce.Video.Height = ushort.Parse(a[1]);

                if (a.Length > 2) /* optional refresh rate */
                    hce.Video.Refresh = ushort.Parse(a[2]);
            }

            if (!string.IsNullOrWhiteSpace(refresh))
                hce.Video.Refresh = ushort.Parse(refresh);

            /**
             * Implicitly invoke the HXE kernel with the HCE loading procedure.
             */

            Run(() => Kernel.Invoke(hce));

            /**
             * This method is used for running code asynchronously and catching exceptions at the highest level.
             */

            void Run(Action action)
            {
                try
                {
                    Task.Run(action).GetAwaiter().GetResult();
                    WithCode(Code.Success);
                }
                catch (Exception e)
                {
                    var msg = " -- EXEC.START HALTED\n Error:  " + e.ToString() + "\n";
                    var log = (File)Paths.Exception;
                    log.AppendAllText(msg);
                    Error(msg);
                    System.Console.Error.WriteLine("\n\n" + e.StackTrace);
                    WithCode(Code.Exception);
                }
            }
        }

        /// <summary>
        ///   Renders a dynamic banner which is both pleasing to the eye and informative.
        /// </summary>
        private static void DisplayBanner()
        {
            bool release = GitVersionInformation.CommitsSinceVersionSource == "0";
            var infoVersion = GitVersionInformation.InformationalVersion;
            string bannerBuildSource = release ? /// TODO: handle pre-releases
                string.Format(BannerBuildSourceRelease, GitVersionInformation.MajorMinorPatch) :
                string.Format(BannerBuildSourceCommit, GitVersionInformation.ShortSha);

            int longestStringLength = GetLongestStringLength(new string[]{
                Banner,
                string.Format(BannerBuildNumber, infoVersion),
                string.Format(BannerBuildSourceCommit, GitVersionInformation.ShortSha),
                string.Format(BannerBuildSourceRelease, GitVersionInformation.MajorMinorPatch)
            });
            var bannerLineDecorations = new string('-', longestStringLength + 1);

            /// Print()
            ForegroundColor = ConsoleColor.Green;      /* the colour of the one */
            WriteLine(Banner);                         /* ascii art and usage */
            WriteLine(BannerBuildNumber, infoVersion); /* reference build */
            WriteLine(bannerLineDecorations);          /* separator */
            WriteLine(bannerBuildSource);              /* reference link */
            WriteLine(bannerLineDecorations);          /* separator */
            ForegroundColor = ConsoleColor.White;      /* end banner */
        }

        internal static int GetLongestStringLength(string[] strings)
        {
            int longestStringLength = 0;
            var lines = new List<string>();

            /// Split strings to individual lines
            foreach (string s in strings)
            {
                foreach (string line in s.Split('\n'))
                {
                    lines.AddRange(line.Split('\r', StringSplitOptions.RemoveEmptyEntries));
                }
            }

            /// Get the length of the longest line
            foreach (string line in lines)
            {
                if (line.Length > longestStringLength)
                {
                    longestStringLength = line.Length;
                }
            }

            return longestStringLength;
        }
    }
}
