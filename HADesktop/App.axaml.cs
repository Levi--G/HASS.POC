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
using Avalonia.Controls;

namespace HADesktop;

public partial class App : Application
{
    HaDesktopApp hassDesktopApp = new();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            DoStartup();
        }
        //else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        //{
        //    singleViewPlatform.MainView = new MainView
        //    {
        //        DataContext = new MainViewModel()
        //    };
        //    singleViewPlatform.MainView.DetachedFromVisualTree += (s, e) => hassDesktopApp.Shutdown();
        //}

        base.OnFrameworkInitializationCompleted();
    }

    void OpenWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow == null)
            {
                var mw = new MainWindow
                {
                    DataContext = new MainViewModel()
                };
                mw.DetachedFromLogicalTree += (s, e) => { (mw.DataContext as MainViewModel)?.Deactivate(); };
                desktop.MainWindow = mw;
            }
            desktop.MainWindow.Show();
            desktop.MainWindow.Activate();
            (desktop.MainWindow.DataContext as MainViewModel)?.Activate();
        }
    }

    async void DoStartup()
    {
        await hassDesktopApp.Startup();
        DataTemplates.Add(AppLocator.Current.GetRequiredService<IntegrationViewLocator>());
        if (!Environment.GetCommandLineArgs().Contains("-m") || AppLocator.Current.GetRequiredService<AppSettingsService>().Settings.IsOnboarding)
        {
            OpenWindow();
        }
    }

    public async void DoShutdown()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mw = desktop.MainWindow;
            if (mw != null)
            {
                mw.Close();
                desktop.MainWindow = null;
            }
            TrayIcon.SetIcons(this, null);
            await hassDesktopApp.Shutdown();
            desktop.Shutdown();
        }
    }

    public void Tray_Click(object? obj, EventArgs args)
    {
        OpenWindow();
    }

    public void Exit_Click(object? obj, EventArgs args)
    {
        DoShutdown();
    }
}
