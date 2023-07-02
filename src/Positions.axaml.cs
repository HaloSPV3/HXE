/**
 * Copyright (c) 2019 Emilian Roman
 * Copyright (c) 2023 Noah Sherwin
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
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MessageBox.Avalonia;
using static HXE.Console;

namespace HXE
{
    /// <summary>
    ///   Interaction logic for Positions.xaml
    /// </summary>
    public partial class Positions : Window
    {
        private static readonly FilePickerFileType _fileType = new("OpenSauce Settings") { Patterns = new string[] { "OS_Settings.User.xml" } };
        private static readonly FilePickerOpenOptions? _openOptions = new()
        {
            Title = "Select OpenSauce Settings File",
            AllowMultiple = false,
            SuggestedStartLocation = null, // assign instanced data
            FileTypeFilter = new FilePickerFileType[] { _fileType },
        };
        private static readonly FilePickerSaveOptions _saveOptions = new()
        {
            FileTypeChoices = new FilePickerFileType[]
            {
                new("OpenSauce Weapon Position Binary")
                { Patterns = new string[] { "*.bin" } }
            },
            DefaultExtension = ".bin",
            ShowOverwritePrompt = true
        };

        public Positions()
        {
            InitializeComponent();
            if (_openOptions is not null)
            {
                _openOptions.SuggestedStartLocation = Task.Run(async () =>
                {
                    string? dir = Path.GetDirectoryName(Paths.HCE.OpenSauce);
                    if (dir is null)
                        ShowError("Avalonia failed to get accessible path for " + dir);
                    else if (GetTopLevel(this) is TopLevel topLevel)
                        return await topLevel.StorageProvider.TryGetFolderFromPathAsync(dir);
                    return null;
                }).Result;
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (SourceTextBox.Text is null || TargetTextBox.Text is null)
                return;

            Info("Saving weapon positions ...");

            var openSauce = (OpenSauce)SourceTextBox.Text;

            if (!openSauce.Exists())
            {
                ShowError("File does not Exist");
                return;
            }

            openSauce.Load();
            openSauce.Objects.Weapon.Save(TargetTextBox.Text);
            openSauce.Objects.Weapon.Load(TargetTextBox.Text);

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
            // writing this without a task wrapping most of it resulted in OpenFilePickerAsync causing a deadlock.
            SourceTextBox.Text = Task.Run(async () =>
            {
                try
                {
                    const int cancelled = 0;
                    var result = await StorageProvider.OpenFilePickerAsync(_openOptions);
                    if (result.Count is not cancelled)
                        return result[0].Path.LocalPath;
                }
                catch (Exception ex) { ShowError(ex.ToString()); }
                return null;
            }).Result;
        }

        private void BrowseTarget(object sender, RoutedEventArgs e)
        {
            TargetTextBox.Text = Task.Run(async () =>
            {
                try
                {
                    IStorageFile? file = await StorageProvider.SaveFilePickerAsync(_saveOptions);

                    if (file is not null)
                    {
                        Debug($"File URI is {file.Path}");
                        Debug($"File's local full path is {file.Path.LocalPath}");
                        return file.Path.LocalPath;
                    }
                }
                catch (Exception ex) { ShowError(ex.ToString()); }
                return null;
            }).Result;
        }

        /// <summary>
        /// Write the string via HXE.Console.Error and print it in a MessageBox window.
        /// </summary>
        /// <param name="v"></param>
        // /// <remarks>Do not invoke from non-UI thread!</remarks>
        private void ShowError(string v) // TODO refactor to public or internal class. I'll probably use it everywhere where I need an error message dialog box.
        {
            MessageBoxManager.GetMessageBoxStandardWindow(new MessageBox.Avalonia.DTO.MessageBoxStandardParams()
            {
                Icon = MessageBox.Avalonia.Enums.Icon.Error,
                ContentTitle = "Error",
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentMessage = v
            }).ShowDialog(this);
        }
    }
}
