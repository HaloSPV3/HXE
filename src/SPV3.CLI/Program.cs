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
using System.Threading.Tasks;
using SPV3.CLI.Exceptions;

namespace SPV3.CLI
{
  /// <summary>
  ///   SPV3.CLI Program.
  /// </summary>
  internal class Program
  {
    /// <summary>
    ///   SPV3.CLI entry.
    /// </summary>
    /// <param name="args">
    ///   Arguments for the CLI.
    /// </param>
    public static void Main(string[] args)
    {
      void Execute()
      {
        try
        {
          Task.Run(() => { Kernel.Bootstrap(); }).GetAwaiter().GetResult();
        }
        catch (AssetException e)
        {
          Console.Error.WriteLine(e.Message);
        }
      }

      void Compile(string source, string target)
      {
        Task.Run(() => { Compiler.Compile(source, target); }).GetAwaiter().GetResult();
      }

      void Install(string source, string target)
      {
        Task.Run(() => { Installer.Install(source, target); }).GetAwaiter().GetResult();
      }

      void PlaceholderCommit(string bitmap, string target, string filter)
      {
        Task.Run(() => { Placeholder.Commit(bitmap, target, filter); }).GetAwaiter().GetResult();
      }

      void PlaceholderRevert(string records)
      {
        Task.Run(() => { Placeholder.Revert(records); }).GetAwaiter().GetResult();
      }

      if (args.Length == 0)
      {
        Execute();
        return;
      }

      if (args[0] == "compile" && args.Length >= 3)
      {
        Compile(args.Length == 2 ? Environment.CurrentDirectory : args[1], args[2]);
        return;
      }

      if (args[0] == "install" && args.Length >= 2)
      {
        Install(args.Length == 2 ? Environment.CurrentDirectory : args[1], args[2]);
        return;
      }

      if (args[0] == "placeholder" && args.Length > 1)
      {
        if (args[1] == "commit" && args.Length >= 4)
          PlaceholderCommit(args[2], args[3], args.Length == 4 ? "*.bmp" : args[4]);
        if (args[1] == "revert" && args.Length >= 2)
          PlaceholderRevert(args[2]);

        return;
      }

      Console.Error.WriteLine("Not enough args.");
    }
  }
}