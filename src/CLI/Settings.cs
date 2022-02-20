using System;
using System.Linq;
using static HXE.Kernel.Configuration;
using static HXE.SettingsCore;

namespace HXE.CLI
{
    public class Settings
    {
        public Settings(SettingsCore core)
        {
            Core = core ?? throw new ArgumentNullException(nameof(core));
        }

        private const string LockedMsg = "(LOCKED BY RULESET)";
        private bool ExitScheduled { get; set; }
        public SettingsCore Core { get; }
        private void InvokeMode(string input)
        {
            /** No Mode entered; print current and available values e.g.
             * SELECT CONFIGURATION MODE
             * Current: SPV33
             * Available:
             * HCE
             * SPV32
             * SPV33
             */
            if (string.IsNullOrEmpty(input))
            {
                Console.Info("Current: " + Core.Configuration.Mode);
                Console.Info("Available:");
                if (ModeUnlocked)
                {
                    foreach (ConfigurationMode mode in Enum.GetValues(typeof(ConfigurationMode)).Cast<ConfigurationMode>().ToList())
                    {
                        Console.Info(mode.ToString());
                    }
                }
                else
                {
                    Console.Warn(LockedMsg);
                }
            }
            // Mode entered; set Mode or warn
            // TODO: move Lock mechanism to Mode's Set accessor
            else if (ModeUnlocked)
            {
                try
                {
                    Core.Configuration.Mode = Enum.GetValues(typeof(ConfigurationMode))
                        .Cast<ConfigurationMode>()
                        .ToList()
                        .Single(m => m.ToString() == input);
                }
                catch (InvalidOperationException)
                {
                    Console.Info("The input " + input + " did not match any Modes");
                }
                catch (Exception ex)
                {
                    Console.Error("Failed to assign Mode");
                    Console.Error(ex.ToString());
                }
            }
            else
            {
                Console.Warn(LockedMsg);
            }
        }

        private void InvokeMain(string input)
        {
            // TODO: MainReset, MainPatch, MainStart, MainResume, MainElevated
            var subcommands = new CommandSet("Main")
            {
                { "Reset", "If True, kill existing haloce processes before starting a new one", s => invokeMainReset(s) },
                { "Patch", "If True, applying patches to haloce.exe before starting it", s => invokeMainPatch(s) },
                { "Resume", "If True, allow Continue button to work by writing last-played map and difficulty to init.txt/initc.txt", s => invokeMainResume(s) },
                { "Start", "If True, start the game after exiting Settings CLI", s => invokeMainStart(s) }
            };

            /** no value entered; print helper */
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.Info("TODO");
            }

            _ = subcommands.Run(input.Split(' '));

            void notImplemented()
            {
                Console.Error("TODO");
                throw new NotImplementedException();
            }

            /// TODO
            void invokeMainReset(string s)
            {
                notImplemented();
            }

            void invokeMainPatch(string s)
            {
                /** no value entered; print helper */
                if (string.IsNullOrWhiteSpace(s))
                {
                    Console.Info("Toggle haloce.exe patching");
                    Console.Info("Current: " + Core.Configuration.Main.Patch.ToString());
                    Console.Info("Available: " + true.ToString() + false.ToString());
                }
            }

            /// TODO
            void invokeMainResume(string s)
            {
                notImplemented();
            }

            /// TODO
            void invokeMainStart(string s)
            {
                notImplemented();
            }
        }

        private void InvokeTweaks(string s) => throw new NotImplementedException();

        private void InvokeVideo(string s) => throw new NotImplementedException();

        private void InvokeAudio(string s) => throw new NotImplementedException();

        private void InvokeInput(string s) => throw new NotImplementedException();



        /// <summary>
        /// Prompt user for the setting to read or write
        /// </summary>
        public void Invoke()
        {
            var Commands = new CommandSet("Settings")
            {
                { "Exit", "Exit the program", s => ExitScheduled = s != null },
                { "Mode", "Select the appropriate Configuration Mode for Halo:CE or your standalone mod", s => InvokeMode(s) },
                { "Main", "Settings of the pre-launch sequence", s => InvokeMain(s) },
                { "Tweaks", "Miscellaneous settings that affect gameplay, user experience, or aesthetics", s => InvokeTweaks(s) },
                { "Video", "Halo:CE's built-in video settings and enhancements provided by HXE", s => InvokeVideo(s) },
                { "Audio", "Halo:CE's built-in audio settings and some misc settings provided by HXE", s => InvokeAudio(s) },
                { "Input", "Gameplay, menu, and auxilliary input settings", s => InvokeInput(s) }
            };

            Console.Info("A CLI tool to change HXE's settings");
            // TODO: allow user to pass Configuration object or path via startup args. See HXE.Program.InvokeProgram()
            while (!ExitScheduled)
            {
                try
                {
                    /// Read input for command name
                    string input = System.Console.In.ReadLine();
                    if (!string.IsNullOrEmpty(input))
                    {
                        Console.Wait("Processing command...");
                        _ = Commands.Run(input.Split(' '));
                    }
                }
                catch (Exception e)
                {
                    Console.Error(e.ToString());
                }
            }
        }
    }
}
