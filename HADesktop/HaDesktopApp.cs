using HADesktop.Extensions;
using HADesktop.Integrations;
using HADesktop.Integrations.Mouse;
using HADesktop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HADesktop
{
    public class HaDesktopApp
    {
        void RegisterAppServices()
        {
            //load general services here
            AppLocator.CurrentMutable.RegisterConstant(this);
            //AppLocator.CurrentMutable.RegisterConstant(new ConfigurationService());
            AppLocator.CurrentMutable.RegisterConstant(new AppSettingsService());
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

        async Task StartAppServices()
        {
            AppLocator.Current.GetRequiredService<AppSettingsService>().Load();
            if (!AppLocator.Current.GetRequiredService<AppSettingsService>().Settings.IsOnboarding)
            {
                await AppLocator.Current.GetRequiredService<MQTTService>().StartConnect();
                await AppLocator.Current.GetRequiredService<MQTTService>().ConnectNow();
            }
        }

        void LoadAppIntegrations()
        {
            AppLocator.Current.GetRequiredService<AppSettingsService>().LoadIntegrations(AppLocator.Current.GetServices<IIntegrationRegistration>().ToList());
        }

        void StartAppIntegrations()
        {
            AppLocator.Current.GetRequiredService<AppSettingsService>().GetIntegrations().ForEach(i => i.Activate());
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

        async Task StopAppServices()
        {
            //stop mqtt etc
            await AppLocator.Current.GetRequiredService<MQTTService>().Disconnect();
        }

        void DisposeAppServices()
        {
            AppLocator.Current.GetService<SchedulingService>()?.Dispose();
        }

        public async Task Startup()
        {
            RegisterAppServices();
            RegisterAppIntegrations();
            await StartAppServices();
            LoadAppIntegrations();
            StartAppIntegrations();
            await AppLocator.Current.GetRequiredService<HomeAssistantService>().SendDiscovery();
            //start integrations with running
            AppLocator.Current.GetService<SchedulingService>()?.Start();
        }

        public async Task StartupInDesignMode()
        {
            RegisterAppServices();
            RegisterAppIntegrations();
            AppLocator.Current.GetRequiredService<AppSettingsService>().Load();
        }

        public async Task Shutdown()
        {
            StopAppIntegrations();
            await StopAppServices();
            DisposeAppServices();
        }
    }
}
