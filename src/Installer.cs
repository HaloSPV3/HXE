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
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HXE.Properties;
#if WINDOWS || NET462 || NET48
using Microsoft.Win32.SafeHandles;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;
#endif
using static System.IO.File;
using static System.IO.Path;
using static HXE.Console;

namespace HXE
{
    /// <summary>
    ///   Installs packages to the filesystem.
    /// </summary>
    public static class Installer
    {
        /// <summary>
        ///   Installs packages from the source directory to the target directory on the filesystem.
        /// </summary>
        /// <param name="source">
        ///   Absolute path to directory containing the installation packages and manifest.
        /// </param>
        /// <param name="target">
        ///   Target directory to install the data from the packages to.
        /// </param>
        /// <param name="progress">
        ///   Optional IProgress object for calling GUI clients.
        /// </param>
        public static void Install(string source, string target, IProgress<Status>? progress = null, bool enableLZNT1 = false)
        {
            /**
             * Normalisation of the paths will preserve our sanity later on! ^_^
             */

            source = GetFullPath(source);
            target = GetFullPath(target);

            Info("Normalised inbound source and target paths");

            Debug("Source - " + source);
            Debug("Target - " + target);

            if (!Directory.Exists(source))
                throw new DirectoryNotFoundException("Source directory does not exist");

            Info("Source directory exists");

            if (!Directory.Exists(target))
                Directory.CreateDirectory(target);
            if (enableLZNT1) /// TODO: refactor to new Method for use from other Classes.
            {
                IEnumerable<string> failedPaths = TransparentlyCompressPaths(target, recurse: true, progress);
                Debug($"The following {failedPaths.Count()} paths failed to compress:\n{string.Join("\n", failedPaths)}");
            }

            Info("Gracefully created target directory");

            var manifest = (Manifest)Combine(source, Paths.Manifest);

            if (!manifest.Exists())
                throw new FileNotFoundException("Manifest file does not exist in the source directory.");

            Info("Manifest binary exists");

            manifest.Load();

            Info("Loaded manifest binary - verifying manifest packages");

            /**
             * Installation is the reversal of the COMPILER routine: we get the data back from the DEFLATE packages, through
             * the use of the generated manifest, and inflate it to the provided target directory on the filesystem.
             */

            foreach (var package in manifest.Packages)
            {
                if (package.Name == null) throw new NullReferenceException("A package's Name is unset!");

                /**
                 * Given that the package filename on the filesystem is expected to match the package's name in the manifest, we
                 * infer the package's path by combining the source with the aforementioned name.
                 */

                var archive = Combine(source, package.Name);

                if (!File.Exists(archive))
                    throw new FileNotFoundException("Package does not exist in the source directory - " + package.Name);

                Info("Package exists - " + package.Name);
            }

            var c = 1;                       /* current package */
            var t = manifest.Packages.Count; /* total progress  */

            foreach (var package in manifest.Packages)
            {
                if (package.Name == null) throw new NullReferenceException("A package's Name (e.g. 1a2b3c4d.bin) is unset!");
                if (package.Entry.Name == null) throw new NullReferenceException("A package's Entry Name (the unpacked file name) is unset!");

                progress?.Report(new Status
                {
                    Current = c - 1,
                    Total = t,
                    Description = $"Installing: {package.Name} - {package.Entry.Name}"
                });

                var archive = Combine(source, package.Name);
                var directory = Combine(target, package.Entry.Path);
                var file = Combine(directory, package.Entry.Name);

                if (File.Exists(file))
                {
                    Delete(file);
                    Info("Deleted existing file - " + file);
                }

                Directory.CreateDirectory(directory);

                Info("Gracefully created directory - " + package.Entry.Path);

                var task = new Task(() => { ZipFile.ExtractToDirectory(archive, directory); });

                /**
                 * While the task is running, we inform the user that is indeed running by updating the console. Aren't we nice
                 * people?
                 */

                task.Start();

                Wait($"Started package inflation - [{(c * 200 + t) / (t * 2):D3}%] - {package.Name} - {package.Entry.Name} ");

                while (!task.IsCompleted)
                {
                    System.Console.Write(Resources.Progress);
                    Thread.Sleep(1000);
                }

                if (File.Exists(file))
                    Info("Entry size - " + new FileInfo(file).Length);
                else
                    Error("Could not get entry size - " + package.Entry.Name);

                Info("Successfully finished package inflation");

                c++;
            }

            manifest.CopyTo(target);

            Info("Copied manifest to the target directory - " + target);
            Done("Installation routine has been successfully completed");
        }

        /// <summary>Apply the filesystem's transparent compression to the given paths<br/>
        /// - block compression is inapplicable<br/>
        /// - copy-on-write links may be broken</summary>
        /// <returns>returns paths that failed to compress</returns>
        /// <todo>
        /// https://btrfs.readthedocs.io/en/latest/Compression.html<br/>
        /// </todo>
        private static IEnumerable<string> TransparentlyCompressPaths(string rootPath, bool recurse, IProgress<Status>? progress = null)
        {
            string[] paths = File.Exists(rootPath)
                ? [rootPath]
                : Directory.GetFileSystemEntries(
                    rootPath,
                    "*",
                    recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly
                );
            Status status = new()
            {
                Description = $"Transparently compressing {paths.Length} directories and files...",
                Total = paths.Length
            };
            progress?.Report(status);
            DriveInfo? targetDrive = null;
            int longestNameLength = 0;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType != DriveType.Fixed)
                    continue;
                /* drive.Name == GetPathRoot(target)! makes sense on Windows, but not on
                  Unix-like platforms where rootpath is always /, but all
                  drive names start with it. */
                if (rootPath.StartsWith(drive.Name) && drive.Name.Length > longestNameLength)
                {
                    targetDrive = drive;
                    longestNameLength = drive.Name.Length;
                }
            }

            if (targetDrive == null)
                return paths; // no matching drive???

            // if the filesystem is read-only or does not support transparent compression, return early
            {
                // writeable transparent-compression filesystems (no SquashFS)
                string[] transparentCompressionFilesystems = [
                    "bcachefs", // gzip, lz4, zstd | Linux
                        "btrfs", // lzo, zlib, zstd | Linux, ReactOS, Windows
                        "f2fs", // lzo, lz4, zstd
                        "ntfs", // lznt1 (lz77) | Windows, MacOS, Linux, FreeBSD, NetBSD, OpenBS, ChromeOS, Solaris, ReactOS (read-only)
                        "reiserfs", // lzo, zlib
                        "zfs" // gzip, lz4, lzjb, zstd
                ];
                if (!transparentCompressionFilesystems.Contains(targetDrive.DriveFormat.ToLowerInvariant()))
                    return paths; // filesystem (probably) does not support transparent compression
            }

            // https://learn.microsoft.com/windows/win32/api/ioapiset/nf-ioapiset-deviceiocontrol
            // https://learn.microsoft.com/openspecs/windows_protocols/ms-fsa/8e2a2e1e-5a90-4251-8b4d-18f1a4c0be43
            List<string> failedPaths = [];
            foreach (string path in paths)
            {
                try
                {
                    bool success = false;
                    bool isDirectory = (GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
#if WINDOWS || NET462 || NET48
                    using SafeFileHandle safeFileHandle = PInvoke.CreateFile(
                        lpFileName: path,
                        dwDesiredAccess: (uint)(GENERIC_ACCESS_RIGHTS.GENERIC_READ | GENERIC_ACCESS_RIGHTS.GENERIC_WRITE),
                        dwShareMode: FILE_SHARE_MODE.FILE_SHARE_READ,
                        lpSecurityAttributes: null,
                        dwCreationDisposition: FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                        dwFlagsAndAttributes: isDirectory
                            ? FILE_FLAGS_AND_ATTRIBUTES.FILE_FLAG_BACKUP_SEMANTICS
                            : FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL
                    );

                    unsafe
                    {
                        success = PInvoke.DeviceIoControl(
                            hDevice: safeFileHandle,
                            dwIoControlCode: PInvoke.FSCTL_SET_COMPRESSION,
                            lpInBuffer: [1] // 0 == None; 1 == Default/LZNT1
                        );
                    }
#endif
                    if (!success) failedPaths.Add(path);
                }
                catch
                {
                    failedPaths.Add(path);
                }
            }
            return failedPaths;
        }
    }
}
