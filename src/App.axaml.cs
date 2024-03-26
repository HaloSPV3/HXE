using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Reactive;
using HotAvalonia;
using ReactiveUI;


namespace HXE;

public class App : Application
{
    /// <summary>Null when not initialized or when Avalonia.Application.Current is not HXE.App</summary>
    public static new App? Current => Application.Current as App;

    public override void Initialize()
    {
        this.EnableHotReload();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow ??= new MainWindow();
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            singleView.MainView = new MainWindow();

        // Here we subscribe to ReactiveUI default exception handler to avoid app
        // termination in case if we do something wrong in our view models. See:
        // https://www.reactiveui.net/docs/handbook/default-exception-handler/
        //
        // In case if you are using another MV* framework, please refer to its
        // documentation explaining global exception handling.
        RxApp.DefaultExceptionHandler = new AnonymousObserver<Exception>(ex => Console.Error(ex.ToString()));

        base.OnFrameworkInitializationCompleted();
    }
}
