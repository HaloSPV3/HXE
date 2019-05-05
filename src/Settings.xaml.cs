using System.Windows;

namespace HXE
{
  /// <summary>
  /// Interaction logic for Settings.xaml
  /// </summary>
  public partial class Settings
  {
    private readonly Configuration _configuration = (Configuration) Paths.Files.Configuration;

    public Settings()
    {
      InitializeComponent();

      if (!_configuration.Exists())
        _configuration.Save();

      _configuration.Load();

      EnableSpv3KernelMode.IsChecked = _configuration.Kernel.EnableSpv3KernelMode;
      EnableSpv3LegacyMode.IsChecked = _configuration.Kernel.EnableSpv3LegacyMode;
      SkipVerifyMainAssets.IsChecked = _configuration.Kernel.SkipVerifyMainAssets;
      SkipInvokeCoreTweaks.IsChecked = _configuration.Kernel.SkipInvokeCoreTweaks;
      SkipResumeCheckpoint.IsChecked = _configuration.Kernel.SkipResumeCheckpoint;
      SkipSetShadersConfig.IsChecked = _configuration.Kernel.SkipSetShadersConfig;
      SkipInvokeExecutable.IsChecked = _configuration.Kernel.SkipInvokeExecutable;
      SkipPatchLargeAAware.IsChecked = _configuration.Kernel.SkipPatchLargeAAware;
    }

    private void Save(object sender, RoutedEventArgs e)
    {
      _configuration.Kernel.EnableSpv3KernelMode = EnableSpv3KernelMode.IsChecked == true;
      _configuration.Kernel.EnableSpv3LegacyMode = EnableSpv3LegacyMode.IsChecked == true;
      _configuration.Kernel.SkipVerifyMainAssets = SkipVerifyMainAssets.IsChecked == true;
      _configuration.Kernel.SkipInvokeCoreTweaks = SkipInvokeCoreTweaks.IsChecked == true;
      _configuration.Kernel.SkipResumeCheckpoint = SkipResumeCheckpoint.IsChecked == true;
      _configuration.Kernel.SkipSetShadersConfig = SkipSetShadersConfig.IsChecked == true;
      _configuration.Kernel.SkipInvokeExecutable = SkipInvokeExecutable.IsChecked == true;
      _configuration.Kernel.SkipPatchLargeAAware = SkipPatchLargeAAware.IsChecked == true;

      _configuration.Save();

      Exit.WithCode(Exit.Code.Success);
    }

    private void Cancel(object sender, RoutedEventArgs e)
    {
      Exit.WithCode(Exit.Code.Success);
    }
  }
}