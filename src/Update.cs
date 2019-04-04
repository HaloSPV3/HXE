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
using System.Collections.Generic;
using System.IO;
using System.Net;
using static System.Diagnostics.Process;
using static System.Environment;
using static System.IO.Compression.ZipFile;
using static System.IO.File;
using static System.IO.Path;

namespace SPV3.CLI
{
  /// <summary>
  ///   Conducts self-updating mechanism for the loader.
  /// </summary>
  public static class Update
  {
    private const string Header  = @"https://open.n2.network/spv3.cli/HEADER.txt"; /* File declaring latest revision. */
    private const string Base    = @"https://open.n2.network/spv3.cli/";           /* Base URL for compiled archives. */
    private const string Archive = @"/bin.zip";                                    /* Archive with compiled binaries. */
    private const string Binary  = @"SPV3.CLI.exe";                                /* Binary in the compiled archive. */
    private const string Type    = @"Release";                                     /* Compiled binary release type.   */

    /// <summary>
    ///   Finish the update routine, by deleting the obsolete executable.
    /// </summary>
    public static void Finish()
    {
      Delete(Combine(CurrentDirectory, ".SPV3.CLI.exe"));
    }

    /// <summary>
    ///   Updates the current executable to the latest one on the server.
    /// </summary>
    public static void Commit()
    {
      using (var response = WebRequest.Create(Header).GetResponse())
      using (var stream = response.GetResponseStream())
      using (var reader = new StreamReader(stream ?? throw new WebException("Could not resolve request.")))
      using (var client = new WebClient())
      {
        /**
         * We first conduct the pre-update clean-up, which consists of removing any files and directories that will be
         * extracted from the archive that will eventually be downloaded.
         *
         * Given that the .NET archive-related classes don't support overwriting upon extraction, we have to delete the
         * relevant files in advance prior to extraction.
         */

        {
          var files       = new List<string> {"COPYRIGHT", "USAGE", "README"};
          var directories = new List<string> {"Release", "Debug"};

          foreach (var file in files)
          {
            if (Exists(file))
              Delete(file);
          }

          foreach (var directory in directories)
          {
            if (Directory.Exists(directory))
              Directory.Delete(directory, true);
          }
        }

        /**
         * Now, we infer the download link for the latest binary, by reading the contents of the header file on the
         * server. Once done, we download the archive to the application data, and extracts its contents to the current
         * directory.
         */

        {
          var hash   = reader.ReadToEnd().TrimEnd('\n');
          var source = new Uri(Base + hash + Archive);
          var target = Combine(GetFolderPath(SpecialFolder.ApplicationData), hash);

          client.DownloadFile(source, target);
          ExtractToDirectory(target, CurrentDirectory);
          Delete(target);
        }

        /**
         * This part is the interesting one. Given that the new executable resides in a subdirectory that was in the
         * downloaded archive, we have to replace the current executable with it. We can't delete the current executable
         * because it's running. However, our workaround is to:
         *
         * 1.  rename the current executable to a different filename; and
         * 2.  rename the new executable to replace the current one; and
         * 3.  instruct the new executable to delete the current executable.
         */

        {
          var source   = Combine(CurrentDirectory, Type, Binary);
          var target   = Combine(CurrentDirectory, GetCurrentProcess().MainModule.FileName);
          var current  = GetCurrentProcess().MainModule.FileName;
          var obsolete = Combine(CurrentDirectory, ".SPV3.CLI.exe");

          Move(current, obsolete);
          Move(source,  target);
          Start(target, "update finish");
          Exit(0);
        }
      }
    }
  }
}