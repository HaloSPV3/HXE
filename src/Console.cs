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
using static System.Console;
using static System.ConsoleColor;

namespace HXE
{
  public static class Console
  {
    public static void Core(string value)
    {
      Output("CORE", Magenta, value, White);
    }

    public static void Error(string value)
    {
      var decoration = new string('*', value.Length);

      Output("!!!!", Red, decoration);
      Output("!!!!", Red, value);
      Output("!!!!", Red, decoration);
    }

    public static void Debug(string value)
    {
      Output("DBUG", Green, value, White);
    }

    public static void Info(string value)
    {
      Output("INFO", Cyan, value, White);
    }

    public static void Wait(string value)
    {
      Output("WAIT", Yellow, value, White);
    }

    public static void Help(string value)
    {
      Output("HELP", Green, value, White);
    }

    public static void Done(string value)
    {
      Output("DONE", Green, value, White);
    }

    public static void Logs(string value)
    {
      Output("LOGS", Magenta, value);
    }

    public static void Warn(string value)
    {
      var decoration = new string('-', value.Length);

      Output("WARN", Yellow, decoration);
      Output("WARN", Yellow, value);
      Output("WARN", Yellow, decoration);
    }

    private static void Output(string prefix, ConsoleColor color, string message)
    {
      Output(prefix, color, message, color);
    }

    private static void Output(
      string       prefix, ConsoleColor color,
      string       message,
      ConsoleColor messageColor,
      bool         writeLine = true
    )
    {
      if (writeLine)
        WriteLine();

      ForegroundColor = Gray;
      Write("> [ ");

      ForegroundColor = color;
      Write(prefix);

      ForegroundColor = Gray;
      Write(" ] - ");

      ForegroundColor = messageColor;
      Write(message);
    }
  }
}
