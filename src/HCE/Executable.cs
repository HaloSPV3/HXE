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

using System.Diagnostics;
using System.IO;
using System.Text;
using static System.Environment;
using static System.IO.Path;
using static HXE.Console;

namespace HXE.HCE
{
  /// <inheritdoc />
  /// <summary>
  ///   Representation of the HCE executable on the filesystem.
  /// </summary>
  public class Executable : File
  {
    public VideoOptions         Video         { get; set; } = new VideoOptions();
    public DebugOptions         Debug         { get; set; } = new DebugOptions();
    public ProfileOptions       Profile       { get; set; } = new ProfileOptions();
    public MiscellaneousOptions Miscellaneous { get; set; } = new MiscellaneousOptions();

    /// <summary>
    ///   Attempt to detect executable on the file-system at the following locations:
    ///   -   current directory; then
    ///   -   default installation paths; then
    ///   -   windows registry; then
    ///   -   hxe installation path
    /// </summary>
    /// <returns>
    ///   Executable object if found.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    ///   Executable has not been found on the file-system.
    /// </exception>
    public static Executable Detect()
    {
      var hce = Detection.Infer();

      if (System.IO.File.Exists(hce.FullName))
        return (Executable) hce.FullName;

      throw new FileNotFoundException("Could not detect executable on the filesystem.");
    }

    public void Start()
    {
      Start(false);
    }

    /// <summary>
    ///   Invokes the HCE executable with the arguments that represent this object's properties' states.
    /// </summary>
    public void Start(bool elevated)
    {
      /**
       * Converts the properties to arguments which the HCE executable can be invoked with.
       */
      string GetArguments()
      {
        var args = new StringBuilder();

        /**
         * Arguments for debugging purposes. 
         */
        if (Debug.Console)
          ApplyArgument(args, "-console ");

        if (Debug.Developer)
          ApplyArgument(args, "-devmode ");

        if (Debug.Screenshot)
          ApplyArgument(args, "-screenshot ");

        if (!string.IsNullOrWhiteSpace(Debug.Initiation))
          ApplyArgument(args, $"-exec \"{GetFullPath(Debug.Initiation)}\" ");

        /**
         * Arguments for video overrides.
         */

        if (Video.Window)
          ApplyArgument(args, "-window ");

        if (Video.NoGamma)
          ApplyArgument(args, "-nogamma ");

        if (Video.Mode)
        {
          if (Video.Width > 0 && Video.Height > 0 && Video.Refresh > 0) /* optional refresh rate */
            ApplyArgument(args, $"-vidmode {Video.Width},{Video.Height},{Video.Refresh} ");
          else if (Video.Width > 0 && Video.Height > 0)
            ApplyArgument(args, $"-vidmode {Video.Width},{Video.Height} ");
        }

        if (Video.Adapter > 1)
          ApplyArgument(args, $"-adapter {Video.Adapter} ");

        /**
         * Argument for custom profile path.
         */

        if (!string.IsNullOrWhiteSpace(Profile.Path))
          ApplyArgument(args, $"-path \"{GetFullPath(Profile.Path)}\" ");

        /**
         * Miscellaneous tweaks. 
         */

        if (Miscellaneous.NoVideo)
          ApplyArgument(args, "-novideo ");

        return args.ToString();
      }

      Info("Starting process for HCE executable");

      Process.Start(new ProcessStartInfo
      {
        FileName = Path,
        WorkingDirectory = GetDirectoryName(Path) ??
                           throw new DirectoryNotFoundException("Failed to infer process working directory."),
        Arguments       = GetArguments(),
        UseShellExecute = true,
        Verb            = elevated ? "runas" : string.Empty
      });

      Info("Successfully started HCE executable");
    }

    private static void ApplyArgument(StringBuilder args, string arg)
    {
      args.Append(arg);
      Debug("Appending argument: " + arg);
    }

    /// <summary>
    ///   Represents the inbound object as a string.
    /// </summary>
    /// <param name="executable">
    ///   Object to represent as string.
    /// </param>
    /// <returns>
    ///   String representation of the inbound object.
    /// </returns>
    public static implicit operator string(Executable executable)
    {
      return executable.Path;
    }

    /// <summary>
    ///   Represents the inbound string as an object.
    /// </summary>
    /// <param name="executable">
    ///   String to represent as object.
    /// </param>
    /// <returns>
    ///   Object representation of the inbound string.
    /// </returns>
    public static explicit operator Executable(string executable)
    {
      return new Executable
      {
        Path = executable
      };
    }

    public class DebugOptions
    {
      public bool   Console    { get; set; }
      public bool   Developer  { get; set; }
      public bool   Screenshot { get; set; }
      public string Initiation { get; set; } = Combine(CurrentDirectory, Paths.HCE.Initiation);
    }

    public class VideoOptions
    {
      public bool   Mode    { get; set; }
      public bool   Window  { get; set; }
      public ushort Width   { get; set; }
      public ushort Height  { get; set; }
      public ushort Refresh { get; set; }
      public byte   Adapter { get; set; }
      public bool   NoGamma { get; set; }
    }

    public class ProfileOptions
    {
      public string Path { get; set; } = Paths.HCE.Directory;
    }

    public class MiscellaneousOptions
    {
      public bool NoVideo { get; set; }
    }
  }
}