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
using System.Text;
using static System.Diagnostics.Process;
using static System.Environment;
using static System.IO.Compression.ZipFile;
using static System.IO.File;
using static System.IO.Path;
using static SPV3.CLI.Console;

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
    ///   Retrieves hash on the remote server.
    /// </summary>
    /// <returns>
    ///   Hash on the remote server.
    /// </returns>
    private static string GetHash()
    {
      Info("Preparing request for latest hash...");

      using (var response = WebRequest.Create(Header).GetResponse())
      using (var stream = response.GetResponseStream())
      using (var reader = new StreamReader(stream ?? throw new WebException("Could not resolve request.")))
      {
        var hash = reader.ReadToEnd().TrimEnd('\n'); /* normalise hash */
        Debug("Inferred hash from server - " + hash);
        return hash;
      }
    }

    /// <summary>
    ///   Verifies if a new update exists.
    /// </summary>
    public static bool Verify()
    {
      /**
       * The update mechanism works by comparing the locally stored hash versus the one returned by the remote server.
       * If the hashes differ, then it's safe to assume that the current executable is out of date. This design choice
       * implicitly encourages regular updates.
       */

      Info("Checking if existing update version binary exists on the filesystem...");

      if (!Exists(Names.Files.UpdateVersion))
      {
        Info("Update version not found. Update is recommended!");
        return true;
      }

      var remoteHash = GetHash();

      using (var fs = new FileStream(Names.Files.UpdateVersion, FileMode.Open, FileAccess.Read))
      using (var ms = new MemoryStream(80))
      using (var br = new BinaryReader(ms))
      {
        fs.CopyTo(ms);
        br.BaseStream.Seek(0x00, SeekOrigin.Begin);

        var localHash  = Encoding.Unicode.GetString(br.ReadBytes(80));
        var newVersion = !remoteHash.Equals(localHash);

        Debug("Current cached local hash - " + localHash);
        return newVersion;
      }
    }

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
      /**
       * We first conduct the pre-update clean-up, which consists of removing any files and directories that will be
       * extracted from the archive that will eventually be downloaded.
       *
       * Given that the .NET archive-related classes don't support overwriting upon extraction, we have to delete the
       * relevant files in advance prior to extraction.
       */

      void Prepare()
      {
        Info("Deleting existing data...");

        var files       = new List<string> {"COPYRIGHT", "USAGE", "README"};
        var directories = new List<string> {"Release", "Debug"};

        foreach (var file in files)
        {
          Debug("Checking if file exists - " + file);

          if (!Exists(file)) continue;

          Delete(file);
          Debug("Deleted existing file - " + file);
        }

        foreach (var directory in directories)
        {
          Debug("Checking if directory exists - " + directory);

          if (!Directory.Exists(directory)) continue;

          Directory.Delete(directory, true);
          Debug("Deleted existing directory - " + directory);
        }

        Info("Finished deleting existing data!");
      }

      /**
       * Now, we infer the download link for the latest binary, by reading the contents of the header file on the
       * server. Once done, we download the archive to the application data, and extracts its contents to the current
       * directory.
       * 
       * For potential subsequent update verification, we store a copy of the hash on the filesystem. This allows
       * the update verification routine to match the hash on the remote server with the locally stored one.
       */

      void Conduct()
      {
        using (var client = new WebClient())
        {
          var hash   = GetHash();
          var source = new Uri(Base + hash + Archive);
          var target = (File) Combine(GetFolderPath(SpecialFolder.ApplicationData), hash);

          Info("Inferred latest binary download URL. Currently downloading...");

          client.DownloadFile(source, target);
          ExtractToDirectory(target, CurrentDirectory);
          target.Delete();

          Info("Downloaded and extracted the latest binary!");

          using (var fs = new FileStream(Names.Files.UpdateVersion, FileMode.OpenOrCreate))
          using (var ms = new MemoryStream())
          using (var bw = new BinaryWriter(ms))
          {
            bw.BaseStream.Seek(0x00, SeekOrigin.Begin);
            bw.Write(Encoding.Unicode.GetBytes(hash));
            ms.WriteTo(fs);
          }
        }
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

      void CleanUp()
      {
        var source   = (File) Combine(CurrentDirectory, Type, Binary);
        var obsolete = (File) Combine(CurrentDirectory, ".SPV3.CLI.exe");
        var current  = (File) Combine(CurrentDirectory, GetCurrentProcess().MainModule.FileName);

        if (obsolete.Exists())
          obsolete.Delete();

        Move(current, obsolete); /* ./SPV3.CLI.exe       => ./.SPV3.CLI.exe */
        Move(source,  current);  /* ./Debug/SPV3.CLI.exe => ./SPV3.CLI.exe  */

        Directory.Delete("Debug",   true);
        Directory.Delete("Release", true);

        Info("Finalising update...");

        Start(current, "update finish");
        Exit(0);
      }

      Prepare();
      Conduct();
      CleanUp();
    }
  }
}