using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Splat;

using HASSPOC.ViewModels;
using HASSPOC.Views;
using HASSPOC.Integrations;
using HASSPOC.Integrations.Mouse;
using HASSPOC.Services;
using System.Reflection;
using System.Linq;
using System;
using HASSPOC.Extensions;

namespace HASSPOC;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        RegisterAppServices();
        RegisterAppIntegrations();

        LoadAppIntegrations();
        StartAppIntegrations();

        DataTemplates.Add(AppLocator.Current.GetRequiredService<IntegrationViewLocator>());

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            desktop.Exit += (s, e) => OnAppShutdown();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
            singleViewPlatform.MainView.DetachedFromVisualTree += (s, e) => OnAppShutdown();
        }

        base.OnFrameworkInitializationCompleted();
    }

    void RegisterAppServices()
    {
        //load general services here

        AppLocator.CurrentMutable.RegisterConstant(new ConfigurationService());
        AppLocator.CurrentMutable.RegisterLazySingleton(() => new MQTTService());
        AppLocator.CurrentMutable.RegisterLazySingleton(() => new HomeAssistantService());
        AppLocator.CurrentMutable.RegisterLazySingleton(() => new LocalizationService());
        AppLocator.CurrentMutable.RegisterLazySingleton(() => new SchedulingService());
        AppLocator.CurrentMutable.RegisterLazySingleton(() => new IntegrationViewLocator());
    }

    void RegisterAppIntegrations()
    {
        //Load all integrations one by one
        //AppLocator.CurrentMutable.RegisterLazySingleton<IIntegration>(() => new MouseIntegration());
        //or (my preference for now)
        MouseIntegration.Register();
        //or (automatic way)
        //var integrationTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IIntegration).IsAssignableFrom(t));
        //foreach (var integrationType in integrationTypes)
        //{
        //    AppLocator.CurrentMutable.RegisterLazySingleton<IIntegration>(() => (IIntegration?)Activator.CreateInstance(integrationType) /*?? throw new NotImplementedException($"Integration {integrationType.FullName} is missing a parameterless constructor")*/);
        //}
        //this can be replaced by a sourcegenerator, real easy to make, so there is 0 extra config.
    }

    void LoadAppIntegrations()
    {
        AppLocator.Current.GetRequiredService<ConfigurationService>().LoadConfigurationFromDisk();
        foreach (var item in AppLocator.Current.GetServices<IIntegration>())
        {
            item.Load();
        }
    }

    void StartAppIntegrations()
    {
        foreach (var item in AppLocator.Current.GetServices<IIntegration>())
        {
            item.Activate();
        }
        //start integrations with running
        AppLocator.Current.GetService<SchedulingService>()?.Start();
    }

    void StopAppIntegrations()
    {
        //stop integrations from running
        AppLocator.Current.GetService<SchedulingService>()?.Stop();

        foreach (var item in AppLocator.Current.GetServices<IIntegration>())
        {
            item.Deactivate();
        }
    }

    void StopAppServices()
    {
        //stop mqtt etc
    }

    void DisposeAppServices()
    {
        AppLocator.Current.GetService<SchedulingService>()?.Dispose();
    }

    void OnAppShutdown()
    {
        StopAppIntegrations();
        StopAppServices();
        DisposeAppServices();
    }
}
