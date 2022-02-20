/**
 * Copyright (c) 2021 Emilian Roman
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

using System.Collections.Generic;
using System.Linq;
using static System.Diagnostics.Process;
using static System.Environment;

namespace HXE
{
    public class Process
    {
        private const string ExceptionHeader = " -- Process Inference failed";
        public enum Type
        {
            Unknown,
            Retail,     /* Halo: Combat Evolved */
            HCE,        /* Halo: Custom Edition */
            Steam,      /* MCC (Steam)          */
            StoreOld,   /* MCC (Windows Store)  */
            Store       /* MCC (Windows Store)  */
        }

        public static IEnumerable<Candidate> Candidates { get; } = new List<Candidate>
        {
          new Candidate { Type = Type.Retail,   Name = "halo"                        },
          new Candidate { Type = Type.HCE,      Name = "haloce"                      },
          new Candidate { Type = Type.Steam,    Name = "MCC-Win64-Shipping"          },
          new Candidate { Type = Type.StoreOld, Name = "MCC-Win64-Shipping-WinStore" },
          new Candidate { Type = Type.Store,    Name = "MCCWinStore-Win64-Shipping"  }
        };

        public static IEnumerable<Result> Results { get; } = new List<Result>
        {
            new Result { Success = true, Message = "Found Halo Retail/CD or Halo Custom Edition"},
            new Result { Success = true, Message = "Found MCC with CEA DLC"},
            new Result { Success = false, Message = "Found MCC, but CEA DLC is missing"},
            new Result { Success = false, Message =
                "No running processes matched the following criteria:" + NewLine +
                "halo.exe v1.0.10.621" + NewLine +
                "haloce.exe v1.0.10.621" + NewLine +
                "MCC-Win64-Shipping.exe with CEA DLC" + NewLine +
                "MCC-Win64-Shipping-WinStore.exe with CEA DLC" + NewLine +
                "MCCWinStore-Win64-Shipping.exe with CEA DLC"},
            new Result { Success = false, Message = "Unknown error occurred."}
        };

        /// <summary>
        ///     Infers the running Halo executable, with support for Halo Retail, Halo Custom Edition, and MCC (Steam & Windows Store).
        /// </summary>
        /// <returns>Type of Platform</returns>
        public static Type Infer()
        {
            Candidate processCandidate = null;
            List<System.Diagnostics.Process> processList = GetProcesses().ToList();
            try
            {
                processCandidate = Candidates.First(x => DeeperCheck(x, processList));
            }
            catch (System.Exception e)
            {
                ErrorOutput(e, "");
            }

            return processCandidate?.Type ?? Type.Unknown;
        }

        private static bool DeeperCheck(Candidate candidate, List<System.Diagnostics.Process> processList)
        {
            System.Diagnostics.Process process;
            try
            {
                process = processList.First(p => p.ProcessName == candidate.Name);
            }
            catch (System.InvalidOperationException)
            {
                return false; /// No processes match current candidate
            }

            switch (process.ProcessName)
            {
                case "halo":
                case "haloce":
                    {
                        try
                        {
                            return process.MainModule.FileVersionInfo.FileVersion == "01.00.10.0621";
                        }
                        catch (System.Exception e)
                        {
                            ErrorOutput(e, "Failed to assess Halo/HaloCE process.");
                            return false;
                        }
                    }

                case string a when a.Contains("MCC") && a.Contains("WinStore"): // redundant, but good practice
                case "MCC-Win64-Shipping-WinStore":
                case "MCCWinStore-Win64-Shipping":
                case "MCC-Win64-Shipping":
                    {
                        try
                        {
                            return process.Modules
                              .Cast<System.Diagnostics.ProcessModule>()
                              .Any(module => module.ModuleName == Paths.MCC.H1dll);
                        }
                        catch (System.Exception e)
                        {
                            string msg2 = "MCC process found, but failed to inspect loaded modules for halo1.dll" + NewLine
                                + (Is64BitProcess ? "Current process is 64-bit." : "Current process is not 32-bit.") + NewLine
                                + (Is64BitOperatingSystem ? "Operating system is 64-bit." : "Operating system is NOT 64-bit.");
                            ErrorOutput(e, msg2);
                            return false;
                        }
                    }

                default:
                    return false;
            }
        }

        private static void ErrorOutput(System.Exception e, string msg2)
        {
            string msg = ExceptionHeader + NewLine
                + msg2 + NewLine
                + "Error:  " + e.ToString();
            ((File)Paths.Exception).AppendAllText(msg + NewLine);
            Console.Error(msg);
        }

        public class Candidate
        {
            public Type Type { get; set; }
            public string Name { get; set; }
        }

        public class Result
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public class ResultAndType
        {
            public Result Result { get; set; }
            public Type Type { get; set; }
        }
    }
}
