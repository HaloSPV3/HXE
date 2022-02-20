/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2022 Noah Sherwin
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
using System.Linq;
using static HXE.Kernel.Configuration;

namespace HXE
{
    /// <summary>
    /// A CLI tool to change HXE's settings
    /// </summary>
    public class SettingsCore
    {
        public Kernel.Configuration Configuration { get; set; } = new Kernel.Configuration();
        public static readonly string ProcessName = System.Diagnostics.Process.GetCurrentProcess().ProcessName.ToLower();
        public static readonly bool ProcessIsSPV3 = ProcessName.Contains("spv3");
        public static readonly bool MainPatchUnlocked = !ProcessIsSPV3;
        public static readonly bool MainResetUnlocked = !ProcessIsSPV3;
        public static readonly bool MainResumeUnlocked = !ProcessIsSPV3;
        public static readonly bool MainStartUnlocked = !ProcessIsSPV3;
        public static readonly bool ModeUnlocked = !ProcessIsSPV3;

        public SettingsCore(Kernel.Configuration cfg)
        {
            Configuration = cfg;
            Initialize();
        }

        /// <summary>
        ///     Load Configuration from file and check Mode for compatibility
        /// </summary>
        /// <remarks>If Mode is incompatible, the Configuration file is reset to default values</remarks>
        private void Initialize()
        {
            Console.Wait("Loading kernel settings...");

            Configuration.Load();

            Console.Info("Kernel settings loaded");

            try { CheckMode(); }
            catch (Exception e)
            {
                Console.Error("Kernel Mode compatibility check failed" + Environment.NewLine + e.Message);
                Console.Wait("Overwriting configuration file with default values...");
                Configuration = new Kernel.Configuration(Configuration.Path);
                Configuration.Save();
                try
                {
                    CheckMode();
                }
                catch (Exception)
                {
                    Console.Error("Something went horribly wrong. HXE tried to reset all Configuration settings, but Mode was still not recognized.");
                }
            }
        }

        /// <summary>
        ///     Check Mode for compatibility
        /// </summary>
        /// TODO: refactor or remove
        public void CheckMode()
        {
            bool valid = Enum.GetValues(typeof(ConfigurationMode)).Cast<ConfigurationMode>().ToList().Exists(m => m == Configuration.Mode);
            if (!valid) throw new Exception("Kernel Mode not recognized");
        }

        /// <summary>
        ///     Print the Configuration settings, descriptions, whether they're locked, and their current value
        /// </summary>
        /// TODO: refactor to output all settings, their description, if they're locked, and their value
        public void PrintConfiguration()
        {
            /* example:
                class Test
                {
                    public string Name { get; set; }
                }

                Test instance = new Test();
                Type type = typeof(Test);

                Dictionary<string, object> properties = new Dictionary<string, object>();
                foreach (PropertyInfo prop in type.GetProperties())
                    properties.Add(prop.Name, prop.GetValue(instance));
            */
            var props = new System.Collections.Generic.Dictionary<string, Type>();
            foreach (System.Reflection.PropertyInfo prop in typeof(Kernel.Configuration).GetProperties())
            {
                props.Add(prop.Name, prop.PropertyType);
            }

            if (ProcessIsSPV3)
            {
                Console.Info("Mode and Main settings are locked by SPV3");
            }
            Console.Info("Mode                             - " + (ModeUnlocked ? "" : "(LOCKED) ") + Configuration.Mode);
            Console.Info("Main   | Kill haloce.exe         - " + (MainResetUnlocked ? "" : "(LOCKED) ") + Configuration.Main.Reset);
            Console.Info("Main   | Patch haloce.exe        - " + (MainPatchUnlocked ? "" : "(LOCKED) ") + Configuration.Main.Patch);
            Console.Info("Main   | Continue SPV3           - " + (MainResumeUnlocked ? "" : "(LOCKED) ") + Configuration.Main.Resume);
            Console.Info("Main   | Start Game              - " + (MainStartUnlocked ? "" : "(LOCKED) ") + Configuration.Main.Start);
            Console.Info("Tweaks | Mouse Acceleration      - " + Configuration.Tweaks.Acceleration);
            Console.Info("Tweaks | Projectile AutoAim      - " + Configuration.Tweaks.AutoAim);
            Console.Info("Tweaks | Cinematic Bars          - " + Configuration.Tweaks.CinemaBars);
            Console.Info("Tweaks | Reticle Magnetism       - " + Configuration.Tweaks.Magnetism);
            Console.Info("Tweaks | Motion Sensor           - " + Configuration.Tweaks.Sensor);
            Console.Info("Tweaks | No PostProcessing       - " + Configuration.Tweaks.Unload);
            Console.Info("Video  | Set Resolution          - " + Configuration.Video.ResolutionEnabled);
            Console.Info("Video  | Disable V-Sync          - " + Configuration.Video.Uncap);
            Console.Info("Video  | Max Quality             - " + Configuration.Video.Quality);
            Console.Info("Video  | Borderless Window       - " + Configuration.Video.Bless);
            Console.Info("Video  | Enable Gamma brightness - " + Configuration.Video.GammaOn);
            Console.Info("Video  | Gamma Level             - " + Configuration.Video.Gamma);
            Console.Info("Audio  | Max Quality             - " + Configuration.Audio.Quality);
            Console.Info("Audio  | Enable EAX              - " + Configuration.Audio.Enhancements); //DevSkim: ignore DS187371
            Console.Info("Input  | Overwrite Controls      - " + Configuration.Input.Override);
        }
    }
}
