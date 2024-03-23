using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace HXE;

public class App : Application
{
    /// <summary>Null when not initialized or when Avalonia.Application.Current is not HXE.App</summary>
    public static new App? Current => Application.Current as App;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow ??= new MainWindow();
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            singleView.MainView = new MainWindow();
        else throw new NotSupportedException($"HXE's {nameof(App)} does not support running {nameof(ApplicationLifetime)} of type '{ApplicationLifetime?.GetType().FullName ?? "<null>"}'.");

        base.OnFrameworkInitializationCompleted();
    }
}
