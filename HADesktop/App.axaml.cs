using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Splat;

using HADesktop.ViewModels;
using HADesktop.Views;
using HADesktop.Integrations;
using HADesktop.Integrations.Mouse;
using System.Reflection;
using System.Linq;
using System;
using HADesktop.Extensions;
using HADesktop.Services;

namespace HADesktop;

public partial class App : Application
{
    HaDesktopApp hassDesktopApp = new();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        hassDesktopApp.Startup();

        DataTemplates.Add(AppLocator.Current.GetRequiredService<IntegrationViewLocator>());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            desktop.Exit += (s, e) => hassDesktopApp.Shutdown();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
            singleViewPlatform.MainView.DetachedFromVisualTree += (s, e) => hassDesktopApp.Shutdown();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
