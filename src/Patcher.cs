using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HXE.Console;

namespace HXE
{
    public class Patcher
    {
        /// <summary>
        /// Read patches.crk to Patches list <br/>
        /// foreach containing exe:haloce.exe, get and apply requested patches. <br/>
        /// if a patch is no longer requested (enabled), restore the original value. <br/>
        /// Patches will be passed to and stored in Kernel via bit-wise integer. <br/>
        /// Later, some patches may be configured in SPV3 loader. <br/>
        /// Perhaps via The "Advanced" menu aka HXE's Configuration UserControl. Bring it full circle. <br/>
        /// </summary>
        public class PatchGroup
        {
            public string Name { get; set; } = string.Empty;   /** Make large address aware */
            public string Executable { get; set; } = string.Empty;   /** haloce.exe               */
            public bool Toggle { get; set; } = false;          /** Patch/Restore values     */
            public List<DataSet> DataSets = new List<DataSet>();
        }

        public class DataSet // 00000136: 0F 2F
        {
            public long Offset { get; set; }
            public byte Original { get; set; }
            public byte Patch { get; set; }

            public static implicit operator DataSet((long Offset, byte Original, byte Patch) v)
                => new() { Offset = v.Offset, Original = v.Original, Patch = v.Patch };
        }

        /// <summary>
        /// The list of patch groups specified by pR0ps' halo ce patches.
        /// </summary>
        private static readonly List<PatchGroup> _patches = //Reader(); // See Write() for tmp overrides
        [
            Patches.Make_large_address_aware,
            Patches.Remove_DRM_and_key_checks,
            Patches.Bind_server_to_0_0_0_0,
            Patches.Disable_safe_mode_prompt,
            Patches.Disable_modification_of_OS_level_gamma,
            Patches.Fix_blue_explosions_when_using_32_bit_textures,
            Patches.Disable_EULA,
            Patches.Disable_storing_exit_status_in_the_registry,
            Patches.Disable_auto_centering_the_crosshair_in_vehicles,
            Patches.Disable_mouse_acceleration,
            Patches.Block_update_checks,
            Patches.Disable_camera_shaking,
            Patches.Prevent_descoping_when_taking_damage,
            Patches.Add_a_link_to_this_repo
        ];

        /// <summary>
        /// Derived from https://github.com/pR0Ps/halo-ce-patches
        /// </summary>
        private static class Patches
        {
            public const string HaloCE_exe = "haloce.exe";
            /// <summary><code>
            /// Make large address aware
            /// haloce.exe
            /// ;-----------------------
            /// ;Normally 32-bit applications can only use up to 2GB of RAM.
            /// ;This patch increases that limit to 4GB.
            /// ;It is required for some maps and mods.
            /// ;
            /// ;Set the IMAGE_FILE_LARGE_ADDRESS_AWARE characteristic flag in the PE header
            /// 00000136: 0F 2F
            /// </code></summary>
            public static readonly PatchGroup Make_large_address_aware = new()
            {
                Name = "Make large address aware",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [(0x136, 0x0F, 0x2F)]
            };
            /// <summary><code>
            /// Remove DRM and key checks
            /// haloce.exe
            /// ;------------------------
            /// ;Allows playing and hosting games without requiring you or connecting players to have valid CD keys.
            /// ;This game was released in 2004 and can no longer be purchased. As a result most current players use
            /// ;cracked copies with one of a few common CD keys. Enforcing unique CD keys on your server will lead
            /// ;to many players being unable to join.
            /// ;
            /// ;Removes the check on startup "Your product key is invalid - we recommend reinstalling the game".
            /// ;Changes a `CMP`, `JZ` to a `JMP` to bypass the check.
            /// 00144C2B: 38 EB
            /// 00144C2C: 18 13
            /// ;
            /// ;Allows a server with a missing/invalid key to host an internet game.
            /// ;`NOP`s out a call to `_atol` to cause a later comparison to fail.
            /// 001BF82A: E8 90
            /// 001BF82B: C7 90
            /// 001BF82C: AC 90
            /// 001BF82D: 00 90
            /// 001BF82E: 00 90
            /// ;
            /// ;Disables checking the CD keys of incoming clients for duplicate keys.
            /// ; Changes a `JE` to a `JMP` to skip the check.
            /// 001BFF48: 74 EB
            /// </code></summary>
            public static readonly PatchGroup Remove_DRM_and_key_checks = new()
            {
                Name = "Remove DRM and key checks",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x144C2B, 0x38, 0xEB),
                    (0x144C2C, 0x18, 0x13),
                    (0x1BF82A, 0xE8, 0x90),
                    (0x1BF82B, 0xC7, 0x90),
                    (0x1BF82C, 0xAC, 0x90),
                    (0x1BF82D, 0x00, 0x90),
                    (0x1BF82E, 0x00, 0x90),
                    (0x1BFF48, 0x74, 0xEB)
                ]
            };

            /// <summary><code>
            /// Bind server to 0.0.0.0
            /// haloce.exe
            /// ;---------------------
            /// ;Fixes LAN game discovery and helps when running on systems with multiple interfaces.
            /// ; Can still bind to a specific IP using the `-ip` flag.
            /// ;
            /// ;The game uses WS2_32.gethostbyname(&lt;your hostname>) to get your IP.
            /// ;This patch takes the result of that call and zeros it so the IP is "0.0.0.0".
            /// ;Replaces `OR ECX,EDX` with `XOR ECX,ECX` to zero out ECX.
            /// 00041695: 0B 31
            /// 00041696: CA C9
            /// </code></summary>
            public static readonly PatchGroup Bind_server_to_0_0_0_0 = new()
            {
                Name = "Bind server to 0.0.0.0",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x41695, 0x0B, 0x31),
                    (0x41696, 0xCA, 0xC9)
                ]
            };
            /// <summary><code>
            /// Disable safe mode prompt
            /// haloce.exe
            /// ;-----------------------
            /// ;Prevents the safe mode prompt from showing and always tries to continue normally. Safe mode is
            /// ;completely broken on modern systems and is pretty easy to accidentally enable from the prompt.
            /// ;If you really want to try safe mode (why?), the `-safemode` launch flag still works.
            //// ;
            //// ;If something went wrong and "safe mode" is an option, just `JMP` to the end of the function.
            //// ;(safe mode is off by default)
            //// 001822D0: 8B E9
            //// 001822D1: 4D 57
            //// 001822D2: 08 02
            //// 001822D3: 51 00
            //// 001822D4: 8B 00
            //// ;
            //// ;Use `XOR EAX,EAX`, `RET` to always return 0 ("continue")
            //// 0018252C: 8B 31
            //// 0018252D: C3 C0
            /// </code></summary>
            public static readonly PatchGroup Disable_safe_mode_prompt = new()
            {
                Name = "Disable safe mode prompt",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x1822D0, 0x8B, 0xE9),
                    (0x1822D1, 0x4D, 0x57),
                    (0x1822D2, 0x08, 0x02),
                    (0x1822D3, 0x51, 0x00),
                    (0x1822D4, 0x8B, 0x00),
                    (0x18252C, 0x8B, 0x31),
                    (0x18252D, 0xC3, 0xC0)
                ]
            };


            /// <summary><code>
            /// Disable modification of OS-level gamma
            /// haloce.exe
            /// ;-------------------------------------
            /// ;In the stock version the current gamma level is saved in the registry on launch and the system-wide
            /// ;gamma level is set to whatever is specified by the in-game setting. When the game exits, the
            /// ;registry key is read and the gamma level is set back. This patch completely removes gamma
            /// ;modification and the associated registry keys.
            /// ;
            /// ;Disable adding the current gamma level to the registry on startup.
            /// ;Replace the first instruction in the function with `RET`.
            /// 00125860: 83 C3
            /// ;
            /// ;Disable in-game setting of gamma.
            /// ;Replace the first instruction in the function with `RET`.
            /// 00125AE0: A0 C3
            /// ;
            /// ;Disable resetting gamma level and setting the registry key when the game exits.
            /// ;Replace the first instruction in the function with `RET`.
            /// 00125A00: 83 C3
            /// </code></summary>
            public static readonly PatchGroup Disable_modification_of_OS_level_gamma = new()
            {
                Name = "Disable modification of OS-level gamma",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x125860, 0x83, 0xC3),
                    (0x125AE0, 0xA0, 0xC3),
                    (0x125A00, 0x83, 0xC3)
                ]
            };

            /// <summary><code>
            /// Fix blue explosions when using 32-bit textures
            /// haloce.exe
            /// ;---------------------------------------------
            /// ;When using maps with uncompressed 32-bit textures, brown explosions would render as blue.
            /// ;Uncompressed 32-bit colors are defined in ARGB but laid out in memory as BGRA due to endianness.
            /// ;Without this patch, only the first 8 bits (the blue channel) were copied and the rest were set to
            /// ;0, causing blue explosions.
            /// ;
            /// ;Replace `MOVZX BYTE` with `MOV DWORD`
            /// 00127C57: 0F 8B
            /// 00127C58: B6 74
            /// 00127C59: 34 3A
            /// 00127C5A: 3A 00
            /// </code></summary>
            public static readonly PatchGroup Fix_blue_explosions_when_using_32_bit_textures = new()
            {
                Name = "Fix blue explosions when using 32-bit textures",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x127C57, 0x0F, 0x8B),
                    (0x127C58, 0xB6, 0x74),
                    (0x127C59, 0x34, 0x3A),
                    (0x127C5A, 0x3A, 0x00)
                ]
            };

            /// <summary><code>
            /// Disable EULA
            /// haloce.exe
            /// ;-----------
            /// ;Allows the game to be run without displaying the EULA. Removes the need for eula.dll to be present.
            /// ;
            /// ;Jump over code that checks for the "FIRSTRUN" registry key and loads eula.dll if it can't find it
            /// ;(eula.dll creates the key).
            /// 0014470D: 8D E9
            /// 0014470E: 55 93
            /// 0014470F: C8 00
            /// 00144710: 52 00
            /// 00144711: 68 00
            /// </code></summary>
            public static readonly PatchGroup Disable_EULA = new()
            {
                Name = "Disable EULA",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x14470D, 0x8D, 0xE9),
                    (0x14470E, 0x55, 0x93),
                    (0x14470F, 0xC8, 0x00),
                    (0x144710, 0x52, 0x00),
                    (0x144711, 0x68, 0x00)
                ]
            };

            /// <summary><code>
            /// Disable storing exit status in the registry
            /// haloce.exe
            /// ;-----------------------------
            /// ;Makes the executable more portable by not requiring/adding registry keys.
            /// ;
            /// ;Disable setting the registry key "ExitFlag" to "clean" when exiting normally.
            /// ;Replace the first instruction in the function with `RET`.
            /// 00182000: 51 C3
            /// ;
            /// ;Bypass accessing the "ExitFlag" registry key on startup to check/set it.
            /// ;Replace the first instruction with `XOR EAX,EAX`, `RET` (return 0).
            /// 00181E40: 81 31
            /// 00181E41: EC C0
            /// 00181E42: 1C C3
            /// </code></summary>
            public static readonly PatchGroup Disable_storing_exit_status_in_the_registry = new()
            {
                Name = "Disable storing exit status in the registry",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x182000, 0x51, 0xC3),
                    (0x181E40, 0x81, 0x31),
                    (0x181E41, 0xEC, 0xC0),
                    (0x181E42, 0x1C, 0xC3)
                ]
            };

            /// <summary><code>
            /// Disable auto-centering the crosshair in vehicles
            /// haloce.exe
            /// ;-----------------------------------------------
            /// ;In the stock version the crosshair in vehicles like the scorpion tank will slowly move towards the
            /// ;center of the screen as you drive. This prevents that behaviour.
            /// ;
            /// ;`NOP` out a `JP` (jump if the parity bit is set).
            /// ;Hypothesis: JP is using the frame count - JP jumps every second frame. By NOPing it out, we never
            /// ;jump and therefore never apply the auto-centering
            /// 0007512A: 7A 90
            /// 0007512B: 0B 90
            /// </code></summary>
            public static readonly PatchGroup Disable_auto_centering_the_crosshair_in_vehicles = new()
            {
                Name = "Disable auto-centering the crosshair in vehicles",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x7512A, 0x7A, 0x90),
                    (0x7512B, 0x0B, 0x90)
                ]
            };

            /// <summary><code>
            /// Disable mouse acceleration
            /// haloce.exe
            /// ;-------------------------
            /// ;These modifications take the built-in mouse acceleration curve and flatten it so no mouse
            /// ;acceleration is applied.
            /// ;
            /// ;Turn a `JLE` into a `JMP`.
            /// ;Skips a loop with some speed calculations and an early return if the speed is under a certain
            /// ;threshold.
            /// 0008F7E2: 7E EB
            /// ;
            /// ;Change an `FADD` to a `FLD` so the constant mouse speed is loaded into `ST0` instead of being
            /// ;added to the previous acceleration calculation.
            /// 0008F826: D8 D9
            /// </code></summary>
            public static readonly PatchGroup Disable_mouse_acceleration = new()
            {
                Name = "Disable mouse acceleration",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x8F7E2, 0x7E, 0xEB),
                    (0x8F826, 0xD8, 0xD9)
                ]
            };

            /// <summary><code>
            /// Block update checks
            /// haloce.exe
            /// ;------------------
            /// ;Prevents checking for game updates. There probably won't be any but if there is you really don't
            /// ;want to be forced to download them. Plus, the check slows down the loading of the server list.
            /// ;
            /// ;Replaces the first instruction of the update check with `XOR EAX,EAX`, `RET` (return 0).
            /// ;A return code of 0 means no updates found.
            /// 0017A920: 55 31
            /// 0017A921: 8B C0
            /// 0017A922: EC C3
            /// </code></summary>
            public static readonly PatchGroup Block_update_checks = new()
            {
                Name = "Block update checks",
                Executable = HaloCE_exe,
                Toggle = true,
                DataSets = [
                    (0x17A920, 0x55, 0x31),
                    (0x17A921, 0x8B, 0xC0),
                    (0x17A922, 0xEC, 0xC3)
                ]
            };

            /// <summary><code>
            /// Disable camera shaking
            /// haloce.exe
            /// ;---------------------
            /// ;Disables the camera shaking that happens when firing weapons or taking damage since it's a bit
            /// ;broken at high framerates.
            /// ;
            /// ;Turn a `JNE` into a `NOP`, `JMP` to skip over the code that applies the shaking.
            /// 000CC46D: 0F 90
            /// 000CC46E: 85 E9
            /// </code></summary>
            public static readonly PatchGroup Disable_camera_shaking = new()
            {
                Name = "Disable camera shaking",
                Executable = HaloCE_exe,
                DataSets = [
                    (0xCC46D, 0x0F, 0x90),
                    (0xCC46E, 0x85, 0xE9)
                ]
            };

            /// <summary><code>
            /// Prevent descoping when taking damage
            /// haloce.exe
            /// ;-----------------------------------
            /// ;Prevents zoomed-in weapons from descoping when the player takes damage.
            /// ;Note that this gives a significant advantage in some situations so it's probably best to only
            /// ;apply and use this if everyone you're playing with does as well.
            /// ;
            /// ;`NOP` out the call to the descope function
            /// 0016B0F1: E8 90
            /// 0016B0F2: 8A 90
            /// 0016B0F3: DD 90
            /// 0016B0F4: FF 90
            /// 0016B0F5: FF 90
            /// </code></summary>
            public static readonly PatchGroup Prevent_descoping_when_taking_damage = new()
            {
                Name = "Prevent descoping when taking damage",
                Executable = HaloCE_exe,
                DataSets = [
                    (0x16B0F1, 0xE8, 0x90),
                    (0x16B0F2, 0x8A, 0x90),
                    (0x16B0F3, 0xDD, 0x90),
                    (0x16B0F4, 0xFF, 0x90),
                    (0x16B0F5, 0xFF, 0x90)
                ]
            };
            /// <summary><code>
            /// Add a link to this repo
            /// haloce.exe
            /// ;----------------------
            /// ;Adds 'github.com/pR0Ps/halo-ce-patches' to an unused section of the executable.
            /// 00000FE0: 00 67
            /// 00000FE1: 00 69
            /// 00000FE2: 00 74
            /// 00000FE3: 00 68
            /// 00000FE4: 00 75
            /// 00000FE5: 00 62
            /// 00000FE6: 00 2e
            /// 00000FE7: 00 63
            /// 00000FE8: 00 6f
            /// 00000FE9: 00 6d
            /// 00000FEA: 00 2f
            /// 00000FEB: 00 70
            /// 00000FEC: 00 52
            /// 00000FED: 00 30
            /// 00000FEE: 00 50
            /// 00000FEF: 00 73
            /// 00000FF0: 00 2f
            /// 00000FF1: 00 68
            /// 00000FF2: 00 61
            /// 00000FF3: 00 6c
            /// 00000FF4: 00 6f
            /// 00000FF5: 00 2d
            /// 00000FF6: 00 63
            /// 00000FF7: 00 65
            /// 00000FF8: 00 2d
            /// 00000FF9: 00 70
            /// 00000FFA: 00 61
            /// 00000FFB: 00 74
            /// 00000FFC: 00 63
            /// 00000FFD: 00 68
            /// 00000FFE: 00 65
            /// 00000FFF: 00 73
            /// </code></summary>
            public static readonly PatchGroup Add_a_link_to_this_repo = new()
            {
                Name = "Add a link to this repo",
                Executable = HaloCE_exe,
                DataSets = [
                    (0x00000FE0, 0x00, 0x67),
                    (0x00000FE1, 0x00, 0x69),
                    (0x00000FE2, 0x00, 0x74),
                    (0x00000FE3, 0x00, 0x68),
                    (0x00000FE4, 0x00, 0x75),
                    (0x00000FE5, 0x00, 0x62),
                    (0x00000FE6, 0x00, 0x2e),
                    (0x00000FE7, 0x00, 0x63),
                    (0x00000FE8, 0x00, 0x6f),
                    (0x00000FE9, 0x00, 0x6d),
                    (0x00000FEA, 0x00, 0x2f),
                    (0x00000FEB, 0x00, 0x70),
                    (0x00000FEC, 0x00, 0x52),
                    (0x00000FED, 0x00, 0x30),
                    (0x00000FEE, 0x00, 0x50),
                    (0x00000FEF, 0x00, 0x73),
                    (0x00000FF0, 0x00, 0x2f),
                    (0x00000FF1, 0x00, 0x68),
                    (0x00000FF2, 0x00, 0x61),
                    (0x00000FF3, 0x00, 0x6c),
                    (0x00000FF4, 0x00, 0x6f),
                    (0x00000FF5, 0x00, 0x2d),
                    (0x00000FF6, 0x00, 0x63),
                    (0x00000FF7, 0x00, 0x65),
                    (0x00000FF8, 0x00, 0x2d),
                    (0x00000FF9, 0x00, 0x70),
                    (0x00000FFA, 0x00, 0x61),
                    (0x00000FFB, 0x00, 0x74),
                    (0x00000FFC, 0x00, 0x63),
                    (0x00000FFD, 0x00, 0x68),
                    (0x00000FFE, 0x00, 0x65),
                    (0x00000FFF, 0x00, 0x73),
                ]
            };
        }

        /// <summary>
        /// Reads PatchGroups from patches.crk to this instance.
        /// </summary>
        /// <returns>A list of PatchGroups read from the patches.crk file resource.</returns>
        /// <remarks>This is only used for _patches list initialization.</remarks>
        public static List<PatchGroup> Reader()
        {
            return _patches;
        }

        /// <summary>
        /// Toggle patches in Halo executable
        /// </summary>
        /// <param name="cfg">HXE.Kernel.Configuration.Tweaks.Patches</param>
        /// <param name="exePath">Path to Halo executable</param>
        public void Write(uint cfg, string exePath)
        {
            var FilteredPatches = new List<PatchGroup>();

            /** Configurable */
            bool DRM = (cfg & EXEP.DISABLE_DRM_AND_KEY_CHECKS) != 0;
            bool NoGamma = (cfg & EXEP.DISABLE_SYSTEM_GAMMA) != 0;
            bool NoAutoCenter = (cfg & EXEP.DISABLE_VEHICLE_AUTOCENTER) != 0;
            bool BlockCamShake = (cfg & EXEP.BLOCK_CAMERA_SHAKE) != 0;
            bool BlockDescopeOnDMG = (cfg & EXEP.PREVENT_DESCOPING_ON_DMG) != 0;

            /** Overrides */
            bool LAA = true;  // (cfg & EXEP.ENABLE_LARGE_ADDRESS_AWARE) != 0;
            bool FixLAN = true;  // (cfg & EXEP.BIND_SERVER_TO_0000)        != 0;
            bool Fix32Tex = true;  // (cfg & EXEP.FIX_32BIT_TEXTURES)         != 0;
            bool NoMouseAccel = false; // (cfg & EXEP.DISABLE_MOUSE_ACCELERATION) != 0;
            bool NoSafe = true;  // (cfg & EXEP.FIX_32BIT_TEXTURES)         != 0;
            bool NoEULA = true;  // (cfg & EXEP.DISABLE_EULA)               != 0;
            bool NoRegistryExit = true;  // (cfg & EXEP.DISABLE_REG_EXIT_STATE)     != 0;
            bool BlockUpdates = true;  // (cfg & EXEP.BLOCK_UPDATE_CHECKS)        != 0;


            /** Filter PatchGroups for those requested
             * NOTE: Update String matches as needed.
             */
            foreach (PatchGroup pg in _patches)
            {
                if (pg == Patches.Make_large_address_aware)
                {
                    pg.Toggle = LAA;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Remove_DRM_and_key_checks)
                {
                    pg.Toggle = DRM;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Bind_server_to_0_0_0_0)
                {
                    pg.Toggle = FixLAN;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Disable_safe_mode_prompt)
                {
                    pg.Toggle = NoSafe;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Disable_modification_of_OS_level_gamma)
                {
                    pg.Toggle = NoGamma;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Fix_blue_explosions_when_using_32_bit_textures)
                {
                    pg.Toggle = Fix32Tex;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Disable_EULA)
                {
                    pg.Toggle = NoEULA;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Disable_storing_exit_status_in_the_registry)
                {
                    pg.Toggle = NoRegistryExit;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Disable_auto_centering_the_crosshair_in_vehicles)
                {
                    pg.Toggle = NoAutoCenter;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Disable_mouse_acceleration)
                {
                    pg.Toggle = NoMouseAccel;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Block_update_checks)
                {
                    pg.Toggle = BlockUpdates;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Disable_camera_shaking)
                {
                    pg.Toggle = BlockCamShake;
                    FilteredPatches.Add(pg);
                    continue;
                }
                if (pg == Patches.Prevent_descoping_when_taking_damage)
                {
                    pg.Toggle = BlockDescopeOnDMG;
                    FilteredPatches.Add(pg);
                    continue;
                }
            }

            /** Flexible patcher */
            {
                bool isFileReady = false;
                while (!isFileReady)
                {
                    try
                    {
                        using (var fs = new FileStream(exePath, FileMode.Open, FileAccess.ReadWrite))
                        using (var ms = new MemoryStream(0x24B000))
                        using (var bw = new BinaryWriter(ms))
                        using (var br = new BinaryReader(ms))
                        {
                            foreach (PatchGroup PatchGroup in FilteredPatches)
                            {
                                foreach (DataSet DataSet in PatchGroup.DataSets)
                                {
                                    byte value = PatchGroup.Toggle ? DataSet.Patch : DataSet.Original;
                                    long offset = DataSet.Offset;
                                    ms.Position = 0;
                                    fs.Position = 0;
                                    fs.CopyTo(ms);

                                    ms.Position = offset;

                                    if (br.ReadByte() != value)
                                    {
                                        if (PatchGroup.Name.Contains("DRM") && !PatchGroup.Toggle)
                                            return;

                                        ms.Position--; /** restore position */
                                        bw.Write(value);  /** write value      */

                                        fs.Position = 0;
                                        ms.Position = 0;
                                        ms.CopyTo(fs);

                                        if (PatchGroup.Toggle)
                                        {
                                            Info($"Applied \"{PatchGroup.Name}\" patch to the HCE executable");
                                        }
                                        else
                                        {
                                            Info($"Removed \"{PatchGroup.Name}\" patch from HCE executable and restored original values.");
                                        }
                                    }
                                    else
                                    {
                                        Info($"HCE executable already patched with \"{PatchGroup.Name}\"");
                                    }
                                }
                            }
                        }

                        isFileReady = true;
                    }
                    catch (IOException)
                    {
                        Wait("Waiting for Halo executable to be available for modification...");
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
        }

        /// <summary>
        /// Offsets for bitwise operations
        /// </summary>
        public static class EXEP
        {
            public const uint ENABLE_LARGE_ADDRESS_AWARE = 1 << 0x00; // Increase max memory range from 2GiB to 4GiB.
            public const uint DISABLE_DRM_AND_KEY_CHECKS = 1 << 0x01; // Removes several DRM/key checks.
            public const uint BIND_SERVER_TO_0000 = 1 << 0x02; // Fixes LAN game discovery. Helps systems with multiple interfaces. Can still bind to IP using `-ip` flag.
            public const uint DISABLE_SAFEMODE_PROMPT = 1 << 0x03; // Disables prompt to restart Halo with disfunctional Safe Mode.
            public const uint DISABLE_SYSTEM_GAMMA = 1 << 0x04; // Disables system-wide modification of display gamma. Disables in-game gamma altogether. Removes need for RegKeys.
            public const uint FIX_32BIT_TEXTURES = 1 << 0x05; // Fixes 32-bit textures being truncated to only a blue channel.
            public const uint DISABLE_EULA = 1 << 0x06; // Allows the game to be run without displaying the EULA. Removes the need for eula.dll to be present.
            public const uint DISABLE_REG_EXIT_STATE = 1 << 0x07; // Makes the executable more portable by not requiring/adding registry keys.
            public const uint DISABLE_VEHICLE_AUTOCENTER = 1 << 0x08; // In stock Halo, the crosshair in vehicles (e.g. scorpion tank) will slowly move towards the horizon as you move.
            public const uint DISABLE_MOUSE_ACCELERATION = 1 << 0x09; // Self-explanatory.
            public const uint BLOCK_UPDATE_CHECKS = 1 << 0x10; // Prevents checking for game updates.
            public const uint BLOCK_CAMERA_SHAKE = 1 << 0x11; // Completely disable camera shake effect. Expose option to players prone to motion sickness.
            public const uint PREVENT_DESCOPING_ON_DMG = 1 << 0x12; // Prevents zoomed-in weapons from descoping when the player takes damage.
            internal const uint ADD_TAG = 1 << 0x13; // pR0Ps' signature
        }
    }
}
