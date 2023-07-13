/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2021 Noah Sherwin
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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Xml.Serialization;
using Avalonia.Controls;
using static System.IO.Compression.CompressionMode;
using static System.Math;
using static System.Text.Encoding;

namespace HXE
{
    /// <summary>
    ///   Object representing an OpenSauce user configuration.
    /// </summary>
    // ?? Should we leverage Microsoft.XmlSerializer.Generator for pre-generated serializers/deserializers?
    [Serializable, DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
    public class OpenSauce : File
    {
        const DynamicallyAccessedMemberTypes DynamicallyAccessedPublicFieldsAndProperties = DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties;
        public OpenSauceCacheFiles CacheFiles { get; set; } = new OpenSauceCacheFiles();
        public OpenSauceRasterizer Rasterizer { get; set; } = new OpenSauceRasterizer();
        public OpenSauceCamera Camera { get; set; } = new OpenSauceCamera();
        public OpenSauceNetworking Networking { get; set; } = new OpenSauceNetworking();
        public OpenSauceHUD HUD { get; set; } = new OpenSauceHUD();
        public OpenSauceObjects Objects { get; set; } = new OpenSauceObjects();

        /// <summary>
        ///   Saves object state to the inbound file.
        /// </summary>
        public void Save()
        {
            using (var writer = new StringWriter())
            {
                var serialiser = new XmlSerializer(typeof(OpenSauce));
                serialiser.Serialize(writer, this);
                WriteAllText(writer.ToString());
            }
        }

        /// <summary>
        ///   Loads object state from the inbound file.
        /// </summary>
        public void Load()
        {
            using (var reader = new StringReader(ReadAllText()))
            {
                var serialiser = new XmlSerializer(typeof(OpenSauce)); // if a FileNotFoundException is thrown, don't worry. It's because we don't pre-gen XML serializers.
                var serialised = (OpenSauce)serialiser.Deserialize(reader);

                CacheFiles = serialised.CacheFiles;
                Rasterizer = serialised.Rasterizer;
                Camera = serialised.Camera;
                Networking = serialised.Networking;
                HUD = serialised.HUD;
                Objects = serialised.Objects;
            }
        }

        /// <summary>
        ///   Represents the inbound object as a string.
        /// </summary>
        /// <param name="openSauce">
        ///   Object to represent as string.
        /// </param>
        /// <returns>
        ///   String representation of the inbound object.
        /// </returns>
        public static implicit operator string(OpenSauce openSauce)
        {
            return openSauce.Path;
        }

        /// <summary>
        ///   Represents the inbound string as an object.
        /// </summary>
        /// <param name="name">
        ///   String to represent as object.
        /// </param>
        /// <returns>
        ///   Object representation of the inbound string.
        /// </returns>
        public static explicit operator OpenSauce(string name)
        {
            return new OpenSauce
            {
                Path = name
            };
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
        public class OpenSauceCacheFiles
        {
            public bool CheckYeloFilesFirst { get; set; } = true;
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
        public class OpenSauceRasterizer
        {
            public RasterizerGBuffer GBuffer { get; set; } = new RasterizerGBuffer();
            public RasterizerUpgrades Upgrades { get; set; } = new RasterizerUpgrades();
            public RasterizerShaderExtensions ShaderExtensions { get; set; } = new RasterizerShaderExtensions();
            public RasterizerPostProcessing PostProcessing { get; set; } = new RasterizerPostProcessing();

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            public class RasterizerGBuffer
            {
                public bool Enabled { get; set; } = true;
            }

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            public class RasterizerUpgrades
            {
                public bool MaximumRenderedTriangles { get; set; } = true;
            }

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            public class RasterizerShaderExtensions
            {
                public bool Enabled { get; set; } = true;
                public ShaderExtensionsObject Object { get; set; } = new ShaderExtensionsObject();
                public ShaderExtensionsEnvironment Environment { get; set; } = new ShaderExtensionsEnvironment();
                public ShaderExtensionsEffect Effect { get; set; } = new ShaderExtensionsEffect();

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class ShaderExtensionsObject
                {
                    public bool NormalMaps { get; set; } = true;
                    public bool DetailNormalMaps { get; set; } = true;
                    public bool SpecularMaps { get; set; } = true;
                    public bool SpecularLighting { get; set; } = true;
                }

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class ShaderExtensionsEnvironment
                {
                    public bool DiffuseDirectionalLightmaps { get; set; } = true;
                    public bool SpecularDirectionalLightmaps { get; set; } = true;
                }

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class ShaderExtensionsEffect
                {
                    public bool DepthFade { get; set; } = true;
                }
            }

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            public class RasterizerPostProcessing
            {
                public PostProcessingMotionBlur MotionBlur { get; set; } = new PostProcessingMotionBlur();
                public PostProcessingBloom Bloom { get; set; } = new PostProcessingBloom();
                public PostProcessingAntiAliasing AntiAliasing { get; set; } = new PostProcessingAntiAliasing();
                public PostProcessingExternalEffects ExternalEffects { get; set; } = new PostProcessingExternalEffects();
                public PostProcessingMapEffects MapEffects { get; set; } = new PostProcessingMapEffects();

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class PostProcessingMotionBlur
                {
                    public bool Enabled { get; set; } = false;
                    public double BlurAmount { get; set; } = 1.00;
                }

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class PostProcessingBloom
                {
                    public bool Enabled { get; set; } = false;
                }

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class PostProcessingAntiAliasing
                {
                    public bool Enabled { get; set; } = false;
                }

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class PostProcessingExternalEffects
                {
                    public bool Enabled { get; set; } = true;
                }

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class PostProcessingMapEffects
                {
                    public bool Enabled { get; set; } = true;
                }
            }
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
        public class OpenSauceCamera
        {
            public double FieldOfView { get; set; } = 70.00;
            public bool IgnoreFOVChangeInCinematics { get; set; } = true;
            public bool IgnoreFOVChangeInMainMenu { get; set; } = true;

            /// <summary>
            /// Reads a display's current resolution and determines the optimal FOV for Halo CE, accounting for the game's FOV calculation quirks.<br/>
            /// </summary>
            /// <param name="window">Optional. An Avalonia window via which the current screens can be enumerated.</param>
            /// <remarks>
            /// If window Avalonia app running is passed while an Avalonia.Application is running, Avalonia will attempt to return the Primary display's resolution or the resolution of the first display it finds.<br/>
            /// If window is null or Avalonia fails, platform-specific operations will try to get the Primary or first-discovered display's display resolution.<br/>
            /// WARNING: if the detected resolution does not match in-game resolution, the FOV will be incorrect due to a bug in Halo: CE's FOV equation.
            /// </remarks>
            /// <returns>The optimal Halo:CE FOV for the Primary (or first-discovered) display's current resolution. If the display resolution does not match the resolution you play with, this FOV will be incorrect.</returns>
            public double CalculateFOV(WindowBase? window = null)
            {
                var (w, h) = (0.0, 0.0);

                if (window is not null && window.Screens.ScreenCount is not 0)
                {
                    // I *could* use tuple assignment for style points, but it doesn't make these assignments any more succinct.
                    // If-Else results in one "is not null" comparison while conditional assignment expressions would result in two comparisons.
                    if (window.Screens.Primary is not null)
                    {
                        w = window.Screens.Primary.Bounds.Width;
                        h = window.Screens.Primary.Bounds.Height;
                    }
                    else
                    {
                        Console.Warn($"Primary display could not be determined. Defaulting to first monitor in index. Identify it by Bounds '{window.Screens.All[0].Bounds}'.");
                        w = window.Screens.All[0].Bounds.Width;
                        h = window.Screens.All[0].Bounds.Height;
                    }
                }
                else if (OperatingSystem.IsWindows())
                {
                    Console.Warn("Avalonia backend is not initialized or its windowing API failed to enumerate connected displays");
                    w = Kernel.GetSystemMetrics(Kernel.SM_CXSCREEN);
                    h = Kernel.GetSystemMetrics(Kernel.SM_CYSCREEN);
                }
                else if (OperatingSystem.IsLinux()) // Developed on Windows. Tested on WSL Ubuntu.
                {
                    string? data = null;
                    using System.Diagnostics.Process xrandr_primary = new()
                    {
                        StartInfo = new()
                        {
                            FileName = "sh",
                            // should print resolution (e.g. "1920x1080") of primary display if its connected
                            Arguments = "-c \"xrandr --current | grep 'connected primary' | grep --extended-regexp --only-matching '[1-9][0-9]+x[1-9][0-9]+' | head -1\"" // will match 10x10 or larger. first line of matches: 1920x1080
                        }
                    };

                    xrandr_primary.OutputDataReceived += (sender, args) => data = args.Data;
                    xrandr_primary.Start();
                    xrandr_primary.BeginOutputReadLine();

                    const uint timeout = 100;
                    for (uint msWaited = 0; data is null && msWaited < timeout; msWaited++)
                        Thread.Sleep(1);

                    if (string.IsNullOrEmpty(data))
                    {
                        xrandr_primary.Kill(true);
                        Console.Warn(
                            $"OS is '{Environment.OSVersion}'.\n" +
                            $"The shell or xrandr failed to respond within {timeout}ms\n" +
                            "-OR- xrandr was not found\n" +
                            "-OR- xrandr did not return data\n" +
                            "-OR- the Primary monitor is not connected\n" +
                            "-OR- none of the connected monitors are marked 'Primary'\n" +
                            "-OR- the Primary monitor's data was printed in a different format than XxY.");

                        /**
                        https://askubuntu.com/a/1351112
                        https://superuser.com/a/603618
                        */
                        using System.Diagnostics.Process xrandr_first = new()
                        {
                            StartInfo = new()
                            {
                                FileName = "sh",
                                Arguments = "-c \"xrandr --current | grep '*' | grep --extended-regexp --only-matching '[1-9][0-9]+x[1-9][0-9]+' | head -1\"",
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                            }
                        };
                        xrandr_first.OutputDataReceived += (sender, args) => data = args.Data;
                        xrandr_first.Start();
                        xrandr_first.BeginOutputReadLine();

                        for (uint msWaited = 0; data is null && msWaited < timeout; msWaited++)
                            Thread.Sleep(1);

                        xrandr_first.Kill(true);

                        if (string.IsNullOrEmpty(data))
                        {
                            throw new TimeoutException($"OS is '{Environment.OSVersion}' and sh/xrandr failed to respond within 100ms or failed to get the resolution of the first connected monitor.");
                        }
                    }
                    else if (data.Trim('0', '1', '2', '3', '4', '5', '6', '7', '8', '9') is "x")
                    {
                        string[] xy = data.Split('x', StringSplitOptions.RemoveEmptyEntries);
                        Console.Info("Attempting to parse data for resolution width and height...");
                        w = double.Parse(xy[0]);
                        h = double.Parse(xy[1]);
                    }
                    else
                    {
                        throw new InvalidOperationException($"grepped string '{data}' did not match expected pattern.");
                    }
                }
                else if (OperatingSystem.IsMacOS())
                {
                    throw new NotSupportedException("This API feature does not yet support Mac OS");
                }

                if (w is 0 || h is 0)
                    throw new InvalidOperationException("Unable to query the current resolution of any display/screen.");

                Console.Info($"Monitor resolution successfully acquired. ({w}x{h})");

                return CalculateFOV(w, h);
            }

            public double CalculateFOV(double width, double height)
            {
                double Degrees(double value)
                {
                    return value * (180 / PI);
                }

                /**
                 * 2 * arctan(((A)/(B)) * tan(C))
                 * Formula by Mortis
                 *
                 * A = New Width / New Height
                 * B = Old Width / Old Height (or ratio) (HCE = 4:3)
                 *
                 * This gets me nostalgic!
                 */

                var w = width - 8;
                var h = height - 8;

                var a = Degrees(w / h);
                var b = Degrees(4 / 3);
                var c = Degrees(70 / 2);

                var x = Atan2(a / b, Tan(c));

                var dirtyResultFix = 9;
                var y = Degrees(2 * x) - dirtyResultFix;

                FieldOfView = Truncate(y * 100) / 100;

                return FieldOfView;
            }
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
        public class OpenSauceNetworking
        {
            public NetworkingGameSpy GameSpy { get; set; } = new NetworkingGameSpy();
            public NetworkingMapDownload MapDownload { get; set; } = new NetworkingMapDownload();
            public NetworkingVersionCheck VersionCheck { get; set; } = new NetworkingVersionCheck();

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            public class NetworkingGameSpy
            {
                public bool NoUpdateCheck { get; set; } = true;
            }

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            public class NetworkingMapDownload
            {
                public bool Enabled { get; set; } = true;
            }

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            public class NetworkingVersionCheck
            {
                public VersionCheckDate Date { get; set; } = new VersionCheckDate();
                public VersionCheckServerList ServerList { get; set; } = new VersionCheckServerList();

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class VersionCheckDate
                {
                    public int Day { get; set; } = (int)DateTime.Now.DayOfWeek;
                    public int Month { get; set; } = DateTime.Now.Month;
                    public int Year { get; set; } = DateTime.Now.Year;
                }

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                public class VersionCheckServerList
                {
                    public int Version { get; set; } = 1;
                    public string Server { get; set; } = "http://os.halomods.com/Halo1/CE/Halo1_CE_Version.xml";
                }
            }
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
        public class OpenSauceObjects
        {
            public bool VehicleRemapperEnabled { get; set; } = true;
            public ObjectsWeapon Weapon { get; set; } = new ObjectsWeapon();

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            public class ObjectsWeapon
            {
                public List<PositionWeapon> Positions { get; set; } = new List<PositionWeapon>();

                /// <summary>
                ///   Loads data from the file specified in the <see cref="Paths.Positions" /> property.
                /// </summary>
                public void Load()
                {
                    Load(Paths.Positions);
                }

                /// <summary>
                ///   Saves data to the file specified in the <see cref="Paths.Positions" /> property.
                /// </summary>
                public void Save()
                {
                    Save(Paths.Positions);
                }

                /// <summary>
                ///   Loads data from the file specified in the inbound path.
                /// </summary>
                /// <param name="path">
                ///   Path of the file.
                /// </param>
                public void Load(string path)
                {
                    using (var inflatedStream = new MemoryStream())
                    using (var deflatedStream = new MemoryStream(System.IO.File.ReadAllBytes(path)))
                    using (var compressStream = new DeflateStream(deflatedStream, Decompress))
                    {
                        compressStream.CopyTo(inflatedStream);
                        compressStream.Close();

                        using (var reader = new StringReader(Unicode.GetString(inflatedStream.ToArray())))
                        {
                            Positions = (List<PositionWeapon>)new XmlSerializer(typeof(List<PositionWeapon>)).Deserialize(reader);
                        }
                    }
                }

                /// <summary>
                ///   Saves data to the file specified in the inbound path.
                /// </summary>
                /// <param name="path">
                ///   Path of the file.
                /// </param>
                public void Save(string path)
                {
                    using (var writer = new StringWriter())
                    {
                        var serialiser = new XmlSerializer(typeof(List<PositionWeapon>));
                        serialiser.Serialize(writer, Positions);

                        using (var deflatedStream = new MemoryStream())
                        using (var inflatedStream = new MemoryStream(Unicode.GetBytes(writer.ToString())))
                        using (var compressStream = new DeflateStream(deflatedStream, Compress))
                        {
                            inflatedStream.CopyTo(compressStream);
                            compressStream.Close();
                            System.IO.File.WriteAllBytes(path, deflatedStream.ToArray());
                        }
                    }
                }

                [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                [XmlType("Weapon")]
                public class PositionWeapon
                {
                    public string Name { get; set; } = string.Empty;
                    public WeaponPosition Position { get; set; } = new WeaponPosition();

                    [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
                    public class WeaponPosition
                    {
                        public double I { get; set; }
                        public double J { get; set; }
                        public double K { get; set; }
                    }
                }
            }
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
        public class OpenSauceHUD
        {
            public bool ShowHUD { get; set; }
            public bool ScaleHUD { get; set; }
            public HUDHUDScale HUDScale { get; set; }

            [DynamicallyAccessedMembers(DynamicallyAccessedPublicFieldsAndProperties)]
            //TODO: rename type
            public class HUDHUDScale
            {
                public double X { get; set; }
                public double Y { get; set; }
            }
        }
    }
}
