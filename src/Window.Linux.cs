#if LINUX
using System;

namespace HXE
{
    [System.Diagnostics.CodeAnalysis.Experimental("HXE9001")]
    public class Window
    {
        public object DataContext { get; set; } = new { };
        public bool? DialogResult { get; set; }

        public void InitializeComponent() { }
        public void Close() { }
    }

    public class RoutedEventArgs : EventArgs { }

    public partial class Settings
    {
        public record struct MiscStruct { public bool IsChecked; public bool IsEnabled; public int SelectedIndex; public string Text; }
        public MiscStruct Mode = new();
        public MiscStruct MainPatch = new();
        public MiscStruct MainReset = new();
        public MiscStruct MainStart = new();
        public MiscStruct MainResume = new();
        public MiscStruct MainElevated = new();
        public MiscStruct TweaksCinemaBars = new();
        public MiscStruct TweaksSensor = new();
        public MiscStruct TweaksMagnetism = new();
        public MiscStruct TweaksAutoAim = new();
        public MiscStruct TweaksAcceleration = new();
        public MiscStruct TweaksUnload = new();
        public MiscStruct VideoAutoResolution = new();
        public MiscStruct VideoUncap = new();
        public MiscStruct VideoQuality = new();
        public MiscStruct VideoBless = new();
        public MiscStruct VideoGammaEnabled = new();
        public MiscStruct VideoGamma = new();
        public MiscStruct AudioQuality = new();
        public MiscStruct AudioEnhancements = new();
        public MiscStruct InputOverride = new();
    }
}
#endif
