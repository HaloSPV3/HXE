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

#if LINUX
using System.Linq;
using System.Threading.Tasks;
using LinuxDesktopUtils.XDGDesktopPortal;
#endif

#if !LINUX
using System.Windows;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;
#endif

using static HXE.Console;

namespace HXE
{
  /// <summary>
  ///   Interaction logic for Positions.xaml
  /// </summary>
#if LINUX
#pragma warning disable HXE9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#endif
  public partial class Positions : Window
#if LINUX
#pragma warning restore HXE9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#endif
  {
    private string _source = string.Empty;
    private string _target = string.Empty;

    public Positions()
    {
      InitializeComponent();
    }

    private void Save(object sender, RoutedEventArgs e)
    {
      Info("Saving weapon positions ...");

      var openSauce = (OpenSauce)_source;

      if (!openSauce.Exists())
      {
#if !LINUX
        MessageBox.Show("Source file does not exist.");
#endif
        Error("Source file does not exist.");
        return;
      }

      openSauce.Load();
      openSauce.Objects.Weapon.Save(_target);
      openSauce.Objects.Weapon.Load(_target);

      foreach (var position in openSauce.Objects.Weapon.Positions)
        Debug($"Weapon: {position.Name} | I/J/K: {position.Position.I}/{position.Position.J}/{position.Position.K}");

      Exit.WithCode(Exit.Code.Success);
    }

    private void Cancel(object sender, RoutedEventArgs e)
    {
      Exit.WithCode(Exit.Code.Success);
    }

    private void BrowseSource(object sender, RoutedEventArgs e)
    {
#if LINUX
      Task<string?> task = Task.Run(static async Task<string?>? () =>
      {
        await using var portal = await DesktopPortalConnectionManager.ConnectAsync();
        var fileChooser = await portal.GetFileChooserPortalAsync();
        var result = await fileChooser.OpenFileAsync(
            dialogTitle: "Select Source File",
            options: new()
            {
              Filters = [
                new() {
                    FilterName = "XML File (*.xml)",
                    IsDefault = true,
                    Patterns = [(GlobPattern)"*.xml"]
                }
              ],
            }
        );

        return ((FileChooserPortal.OpenFileResults?)result.Results)?.SelectedFiles
          .FirstOrDefault()?.AbsolutePath;
      });
      task.Wait();
      if (task.Result is null)
        return;
      _target = task.Result;
#else
      var dialog = new OpenFileDialog
      {
        DefaultExt = ".xml",
        Filter = "XML files (*.xml)|*.xml"
      };

      if (dialog.ShowDialog() != true) return;

      _source = dialog.FileName;
      SourceTextBox.Text = _source;
#endif

      Info($"Positions file: {_source}");
    }

    private void BrowseTarget(object sender, RoutedEventArgs e)
    {
#if LINUX
      Task<string?> task = Task.Run(static async Task<string?>? () =>
      {
        await using var portal = await DesktopPortalConnectionManager.ConnectAsync();
        var fileChooser = await portal.GetFileChooserPortalAsync();
        var result = await fileChooser.SaveFileAsync(
            dialogTitle: "Set File Path",
            options: new()
            {
              SuggestedFileName = "Positions.bin",
              Filters = [
                new() {
                    FilterName = "Binary blob (*.bin)",
                    IsDefault = true,
                    Patterns = [(GlobPattern)"*.bin"]
                }
              ],
            }
        );

        return ((FileChooserPortal.SaveFileResults?)result.Results)
          ?.SelectedFileLocation.AbsolutePath;
      });
      task.Wait();
      if (task.Result is null)
        return;
      _target = task.Result;
#else
      var dialog = new SaveFileDialog
      {
        DefaultExt = ".bin",
        Filter = "BIN files (*.bin)|*.bin"
      };

      if (dialog.ShowDialog() != true) return;

      _target = dialog.FileName;
      TargetTextBox.Text = _target;
#endif
    }

#if LINUX
    /* For internal use in Test.cs or unit tests */
    internal static void Save(Positions positions) => positions.Save(new(), new());
    internal static void Cancel(Positions positions) => positions.Cancel(new(), new());

    internal static void BrowseSource(Positions positions) => positions.BrowseSource(new(), new());
    internal static void BrowseTarget(Positions positions) => positions.BrowseTarget(new(), new());
#endif
  }
}
