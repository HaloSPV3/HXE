using System;
using System.IO;
using HXE.HCE;
using static HXE.Console;
using static HXE.Paths;
using static HXE.NewProfile.GenerateVars;
using Directory = System.IO.Directory;

namespace HXE
{
  /// <inheritdoc />
  /// <summary>
  ///   Object used for creating a new Player Profile.
  /// </summary>
  class NewProfile : File
  {
    /// <summary>
    ///   Call NewProfile.Profile() and assign the generated Profile as the LastProfile.
    /// </summary>
    /// <param name="scaffold"    >Inherit and pass the bool indicating if the scaffold must be created.</param>
    /// <param name="pathParam"   >Inherit and pass the -path parameter.</param>
    /// <param name="lastProfile" >Inherit the instance</param>
    /// <param name="profile"     >Inherit and pass the Profile instance to Profile.Generate().</param>
    public static void LastProfile(bool scaffold, string pathParam, LastProfile lastProfile, Profile profile)
    {
      Core("LastProfile.Generate");
      Profile(scaffold, pathParam, profile);

      lastProfile.Profile = profile.Details.Name;

      using (StreamWriter lastproftxt = System.IO.File.AppendText($"{Custom.LastProfile(pathParam)}"))
        lastProfile.Save();
    }

    /// <summary>
    ///   Create retrievable variables for Profile Generation. Generate a NewXXX-style name for the new Player Profile.
    /// </summary>
    public class GenerateVars
    {
      readonly private static string NameGen = $"New{new Random().Next(1, 999).ToString("D3")}";

      /// <summary>
      ///   Use once to generate a profile name. Read from ProfileName for the result.
      /// </summary>
      public static void SetNewName(Profile profile)
      {
        profile.Details.Name = NameGen;
      }

      /// <summary>
      ///   Assigns $"{UserData}\\savegames\\{ProfileName}\\{ProfileName}" to Waypoint Path.
      /// </summary>
      //public static void SetWaypointPath(Profile profile, string pathParam)
      //{
      //  WaypointPath = Custom.Waypoint(UserData, profile.Details.Name);
      //}

      /// <summary>
      ///   Output the Path variable's current value. Verify its full path exists in the file system.
      /// </summary>
      public static void VerifyPath(string path)
      {
        Info($"Path is currently \"{path}\"");
        Info($"The Full Path is \"{System.IO.Path.GetFullPath(path)}\"");
        Info($"\"{path}\" exists? {Directory.Exists(System.IO.Path.GetFullPath(path))}");
      }
    }

    /// <summary>
    ///   Create file and folder structure for Profile.Generate() using variables from GenVars.
    /// </summary>
    /// <param name="pathParam" >-path parameter to pass to Halo and write to profiles.</param>
    /// <param name="profile"   >Object to represent as string.</param>
    /// <param name="file"      >An instance of the File class</param>
    public static File Scaffold(string pathParam, Profile profile, File file)
    {
      /// and profile files...
      /// e.g., blam.sav, savegame.bin, et cetera
      Core("Creating Scaffold...");

      /// Set Path to the -path parameter
      /// Check if it exists, create it if it doesn't.
      /// Print the full path to the console.
      file.Path = pathParam;  /// e.g. ".\temp\"
      file.CreateDirectory();
      VerifyPath(file.Path);

      /// Set Path to the savegames directory, et cetera
      file.Path = Custom.Profiles(pathParam);  /// e.g. ".\temp\savegames\
      file.CreateDirectory();
      VerifyPath(file.Path);

      /// Set Path to the current Profile's directory, et cetera
      file.Path = Custom.ProfileDirectory(pathParam, profile.Details.Name);  /// e.g. ".\temp\savegames\New001\"
      file.CreateDirectory();
      VerifyPath(file.Path);

      /// Create blam.sav
      using (StreamWriter blam = System.IO.File.AppendText($"{file.Path}\\blam.sav"))
        file.WriteAllBytes(new byte[0x2000]); /// 0x2000 == int 8192

      /// Create savegame.bin
      using (StreamWriter savegame = System.IO.File.AppendText($"{file.Path}\\savegame.bin"))
        file.WriteAllBytes(new byte[0x480000]); /// 0x480000 == int 4718592

      /// Create waypoint
      using (StreamWriter waypoint = System.IO.File.AppendText(Custom.Waypoint(pathParam, profile.Details.Name)))
        file.WriteAllText(Custom.Waypoint(pathParam, profile.Details.Name));
      /// "waypoint" is used to refer to the file at "$Profile\\savegames\\$Details.Name\\$Details.Name"
      /// Halo writes the path of this file as the file's contents.
      /// For instance, `.\profiles\savegames\New001\New001`
      ///   when `-path .\profiles`
      return file;
    }

    /// <summary>
    ///   Generate a new Player Profile. Create savegames folder and relevant file structures. 
    /// </summary>
    /// <param name="scaffold">Boolean to determine whether to execute Scaffold generation.</param>
    /// <param name="path"    >-path parameter to pass to Halo and write to profiles.</param>
    /// <param name="profile" >Object to represent as string.</param>
    public static void Profile(bool scaffold, string path, Profile profile)
    {
      /// todo:
      ///   create the file.
      ///     double check it
      ///   DONE: if the file is still null, *then* throw an exception.
      ///     Do this everywhere this function is called.
      ///   populate with default settings
      ///     blam.sav has some defaults listed. What needs to be set manually?
      ///   load the file
      ///     Do I still need to do this?
      Core("Profile.Generate");
      var file = new File();
      SetNewName(profile);
      profile.Path = Custom.Profile(path, profile.Details.Name);

      if (!scaffold)
        Scaffold(path, profile, file);

      profile.Save();
    }
  }
}
