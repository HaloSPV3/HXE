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
using MessageBox.Avalonia.Enums;
using static HXE.Console;

namespace HXE
{
    /// <summary>
    ///   Interaction logic for Positions.xaml
    /// </summary>
    public partial class Positions : Window
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
                MessageBoxManager
                    .GetMessageBoxStandardWindow(
                        "Error",
                        "Source file does not exist.",
                        ButtonEnum.Ok,
                        MessageBox.Avalonia.Enums.Icon.Error,
                        WindowStartupLocation.CenterOwner)
                    .ShowDialog(this)
                    .Wait();
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
            try
            {
                IStorageFolder? suggestedStartLocation = null;
                try
                {
                    suggestedStartLocation = StorageProvider.TryGetFolderFromPathAsync(Path.GetDirectoryName(Paths.HCE.OpenSauce)).Result;
                }
                catch
                {
                    MessageBoxManager
                    .GetMessageBoxStandardWindow(
                        "Error",
                        $"Failed to find file '{Paths.HCE.OpenSauce}'. It might not exist.")
                    .ShowDialog(this)
                    .Wait();
                }

                try
                {
                    IStorageFile inputFile = StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                    {
                        Title = "Select OpenSauce.User.xml",
                        AllowMultiple = false,
                        SuggestedStartLocation = suggestedStartLocation,
                        FileTypeFilter = new FilePickerFileType[]
                            {
                            new FilePickerFileType("OpenSauce.*.xml")
                            {
                                Patterns = new string[] { "OpenSauce.*.xml" }
                            }
                            },
                    }).Result[0];

                    _source = inputFile.Path.LocalPath;
                    SourceTextBox.Text = _source;
                }
                catch (Exception ex)
                {
                    MessageBoxManager
                        .GetMessageBoxStandardWindow(
                            "FilePicker Unsuccessful",
                            ex.ToString(),
                            ButtonEnum.Ok,
                            icon: MessageBox.Avalonia.Enums.Icon.Error,
                            windowStartupLocation: WindowStartupLocation.CenterOwner)
                        .ShowDialog(this)
                        .Wait();
                }
            }
            catch (Exception ex)
            {
                MessageBoxManager
                    .GetMessageBoxStandardWindow("Error", ex.ToString(), icon: MessageBox.Avalonia.Enums.Icon.Error)
                    .ShowDialog(this)
                    .Wait();
            }
        }

        private void BrowseTarget(object sender, RoutedEventArgs e)
        {
            try
            {
                var types = new FilePickerFileType[]
                {
                    new FilePickerFileType("OpenSauce Weapon Position Binary")
                    {
                        Patterns = new string[] { "*.bin" }
                    }
                };

                IStorageFile? file = StorageProvider
                    .SaveFilePickerAsync(new FilePickerSaveOptions()
                    {
                        FileTypeChoices = types,
                        DefaultExtension = ".bin",
                        ShowOverwritePrompt = true
                    })
                    .Result ?? throw new TaskCanceledException();

                _target = file.Path.LocalPath;
                TargetTextBox.Text = _target;
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                MessageBoxManager
                    .GetMessageBoxStandardWindow(
                        "FilePicker Unsuccessful",
                        ex.ToString(),
                        ButtonEnum.Ok,
                        MessageBox.Avalonia.Enums.Icon.Error,
                        WindowStartupLocation.CenterOwner)
                    .ShowDialog(this)
                    .Wait();
            }
        }
    }
}
