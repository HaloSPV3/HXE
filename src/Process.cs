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
using System.Diagnostics;
using System.Linq;
using static System.Diagnostics.Process;

namespace HXE
{
	public class Process
	{
		public enum Type
		{
			Unknown,
			Retail, /* Halo: Combat Evolved */
			HCE,    /* Halo: Custom Edition */
			Steam,  /* MCC (Steam)          */
			Store   /* MCC (Windows Store)  */
		}

		public static IEnumerable<Candidate> Candidates { get; } = new List<Candidate>
		{
			new Candidate { Type = Type.Retail, Name = "halo"                        },
			new Candidate { Type = Type.HCE,    Name = "haloce"                      },
			new Candidate { Type = Type.Steam,  Name = "MCC-Win64-Shipping"          },
			new Candidate { Type = Type.Store,  Name = "MCC-Win64-Shipping-WinStore" }
		};

		/**
		 * Infers the running Halo executable, with support for HCE, HCE and MCC (Steam & Windows Store).
		 */
		public static Type Infer()
		{
			foreach (var candidate in Candidates)
			{
				var processes = GetProcessesByName(candidate.Name);

				if (processes.Length <= 0)
					continue;

				switch (candidate.Type)
				{
					/**
					 * MCC CEA
					 *
					 * HXE infers a valid MCC process through the invocation of a valid MCC executable with the CEA DLL attached.
					 */
					case Type.Steam:
					case Type.Store:
					{
						var hasCEA = processes
							.First()
							.Modules
							.Cast<ProcessModule>()
							.Any(module => module.ModuleName == Paths.MCC.Halo1dll);

						if (hasCEA)
							return candidate.Type;

						break;
					}

					/**
					 * HPC/HCE
					 *
					 * HXE infers a valid HPC/HCE process through the invocation of a valid executable with the specified version.
					 */
					case Type.Retail:
					case Type.HCE:
					{
						var isValid = processes
							.Any(process => process.MainModule?.FileVersionInfo.FileVersion == "01.00.10.0621");

						if (isValid)
							return candidate.Type;

						break;
					}
				}
			}

			return Type.Unknown;
		}

		public class Candidate
		{
			public Type   Type { get; set; }
			public string Name { get; set; }
		}
	}
}