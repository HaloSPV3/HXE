/**
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
using System.IO;

namespace HXE.CLI
{
    public static class Positions
    {
        public static void Run(string source = null, string target = null)
        {
            Console.Info("Read the file \"OS_Settings.User.xml\" and write its weapons positions to a .bin file.");
            FileInfo fiSource = null;
            FileInfo fiTarget = null;

            while (fiSource == null)
            {
                try
                {
                    fiSource = GetSource();
                }
                catch (Exception e)
                {
                    Console.Error(e.ToString());
                }
            }

            while (fiTarget == null)
            {
                try
                {
                    fiTarget = GetTarget();
                }
                catch (Exception e)
                {
                    Console.Error(e.ToString());
                }
            }

            try
            {
                Save(fiSource, fiTarget);
            }
            catch (Exception e)
            {
                Console.Error(e.ToString());
            }


        }

        private static FileInfo GetSource(string source = null)
        {
            FileInfo fileInfo = null;

            Console.Info("Full path of OS_Settings.User.xml:");

            if (source == null)
            {
                string input = System.Console.In.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new NullReferenceException("The supplied path was null, empty, or whitespace.");
                }

                fileInfo = new FileInfo(Path.GetFullPath(input));
            }
            else
            {
                Console.Info(source);
                fileInfo = new FileInfo(Path.GetFullPath(source));
            }

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"The file {fileInfo.Name} was not found.");
            }

            if (fileInfo.Name != "OS_Settings.User.xml")
            {
                throw new ArgumentException("The provided file is not OS_Settings.User.xml.");
            }

            return fileInfo;
        }

        private static FileInfo GetTarget(string target = null)
        {
            FileInfo fileInfo;

            Console.Info("Full path of target/output .bin file:");

            if (target == null)
            {
                string input = System.Console.In.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    throw new NullReferenceException("The supplied path was null, empty, or whitespace.");
                }

                fileInfo = new FileInfo(Path.GetFullPath(input));
            }
            else
            {
                Console.Info(target);
                fileInfo = new FileInfo(Path.GetFullPath(target));
            }

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"The file {fileInfo.Name} was not found.");
            }

            if (!fileInfo.Extension.EndsWith("bin"))
            {
                throw new ArgumentException("The provided file lacks the .bin extension.");
            }

            return fileInfo;

        }

        private static void Save(FileInfo source, FileInfo target)
        {
            Console.Info("Saving weapon positions...");

            var openSauce = (OpenSauce) source.FullName;

            openSauce.Load();
            openSauce.Objects.Weapon.Save(target.FullName);
            openSauce.Objects.Weapon.Load(target.FullName);

            foreach (var position in openSauce.Objects.Weapon.Positions)
                Console.Debug($"Weapon: {position.Name} | I/J/K: {position.Position.I}/{position.Position.J}/{position.Position.K}");
        }
    }
}
