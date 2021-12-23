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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.IO.Path;

namespace HXE.Steam
{
    public class Libraries : File
    {
        public static List<string> LibList = new List<string>();

        /// <summary>
        ///     Search for LibraryFolders.vdf under a given root directory or Steam's directory.
        /// </summary>
        /// <param name="rootDir">If unspecified, defaults to current value of <c>HXE.Paths.Steam.Directory</c>.</param>
        /// <returns>A list of LibraryFolders.vdf files.</returns>
        static List<FileInfo> FindLibrariesRecursively(DirectoryInfo rootDir = null)
        {
            /// If <c>root</c> is null, check if <c>HXE.Paths.Steam.Directory</c> is null, empty, or whitespace.
            ///     If `HXE.Paths.Steam.Directory` is null, empty, or whitespace, then throw ArgumentNullException.
            ///     Otherwise, assign its value to the `root` parameter as a DirectoryInfo object.
            if (rootDir == null)
            {
                if (string.IsNullOrWhiteSpace(Paths.Steam.Directory))
                {
                    const string msg = "Steam's root directory was not specified.\n"
                        + "The values of both `HXE.Steam.Libraries.FindLibrariesRecursively().root` and `HXE.Paths.Steam.Directory` were not set, are empty, or are only whitespace.";
                    throw new System.ArgumentNullException(nameof(rootDir), msg);
                }
                else
                {
                    rootDir = new DirectoryInfo(Paths.Steam.Directory);
                }
            }

            /// <summary>
            ///     Recursively search the directory indicated via the <c>root</c> variable for "LibraryFolders.vdf".
            /// </summary>
            /// <value>Return a list of located files.</value>
            try
            {
                FileInfo[] searchResults = rootDir.GetFiles("LibraryFolders.vdf", SearchOption.AllDirectories); /// System.IO.DirectoryNotFoundException(), System.Security.SecurityException("The caller does not have the required permission.")
                if (searchResults.Length == 0)
                {
                    throw new FileNotFoundException($"Search succeeded, but no files match 'LibraryFolders.vdf'.");
                }

                return searchResults.ToList();
            }
            catch(System.Exception e)
            {
                throw new System.Exception($"Failed the search for 'LibraryFolders.vdf' in '{rootDir}'. Reason:\n" + e.ToString(), e);
            }
        }

        /// <summary>
        ///     Read Steam's "libraryfolders.vdf" files(s) and assign the library folders to a List.
        /// </summary>
        /// <param name="rootDir">
        ///     The root directory to recursively search for "LibraryFolders.vdf".<br/>
        ///     Default: HXE.Paths.Steam.Directory
        /// </param>
        /// TODO: utilize package 'Gameloop.Vdf'
        public static void ParseLibraries(DirectoryInfo rootDir = null, FileInfo libraryFoldersVdf = null)
        {
            List<FileInfo> libraryFoldersVdfList = FindLibrariesRecursively(rootDir);

            if (libraryFoldersVdf != null)
            {
                /// Add the specified LibraryFolders.vdf file to the beginning of the list.
                var tmp = libraryFoldersVdfList;
                libraryFoldersVdfList = new List<FileInfo> { libraryFoldersVdf };
                libraryFoldersVdfList.AddRange(tmp);
            }

            foreach (var file in libraryFoldersVdfList) /// May throw wrapped exceptions.
            {
                try
                {
                    ParseLibrary(new File { Path = file.Name }.ReadAllText());
                }
                catch (System.Exception e)
                {
                    throw new System.Exception("Failed to Parse Steam Libraries file", e);
                }
            }

            if (LibList.Count == 0)
            {
                /// Severity: WARNING
                throw new System.Exception("One or more files matching 'LibraryFolders.vdf' were found and read, but no paths of libraries were read from the file(s).");
            }
        }

        public static void ParseLibrary(File libraryFoldersVdf = null)
        {
            string text = libraryFoldersVdf.ReadAllText();

            List<string> libs = text.Split("\n").ToList(); /// Start by adding each line to a list.
            libs = libs.Where(line => line.Contains("\"path\"")).ToList(); /// Filter the list for entries containing `"path"`.

            foreach (string line in libs)
            {
                string entry = line; /// Copy the current line to a modifiable variable named `entry`.
                entry = entry.Replace("\"path\"", ""); /// remove `"path"` from the entry, leaving only the value of `path` enclosed by double quotes.
                entry = entry.Trim(); /// remove whitespace from the start and end of the string.
                entry = entry.Replace("\"", ""); /// Assuming the value of `path` is the remainder, strip the enclosing double quotes.
                if (!string.IsNullOrWhiteSpace(entry)) /// If the remainder is not null, empty, or whitespace, add it to the main Library list.
                {
                    LibList.Add(entry);
                }
            }
        }

        /// <summary>
        /// Search each discovered Steam Library for a given path. <br/>
        /// Each result is assigned to the ReturnPaths array.
        /// </summary>
        /// <param name="pathInLibrary">
        /// The path to find within the Steam Library folders.
        /// </param>
        /// <remarks>
        /// If only one result is expected, it can be accessed as ReturnPath[0]. <br/>
        /// If there are multiple results, implement another While loop to filter them.
        /// </remarks>
        public List<string> FindInLibraries(string pathInLibrary)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (string library in LibList)
                {
                    string path = Combine(library, "steamapps", "common", pathInLibrary);
                    if (System.IO.File.Exists(path))
                        list.Add(path);
                }
            }
            catch (System.Exception e)
            {
                throw new System.Exception($"Failed to find {pathInLibrary} in Steam Libraries.", e);
            }

            if (list.Count == 0)
                throw new FileNotFoundException($"Failed to find {Path} in Steam Libraries.");

            return list;
        }
    }
}
