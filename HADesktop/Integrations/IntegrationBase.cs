using Avalonia.Controls;
using HADesktop.Extensions;
using HADesktop.Services;

namespace HADesktop.Integrations
{
    abstract internal class IntegrationBase<ConfigurationType, ViewModelType> : IIntegration
        where ConfigurationType : IConfigurationObject, new()
        where ViewModelType : IIntegrationViewModel, new()
    {
        public string Name => LocalizationService.GetLocalizedByKey(LocalizationKey + ".Name");
        public string Description => LocalizationService.GetLocalizedByKey(LocalizationKey + ".Description");
        protected abstract string LocalizationKey { get; }
        public abstract string Code { get; }
        public IIntegrationViewModel ConfigurationViewModel => new ViewModelType() { Integration = this, Configuration = configuration };

        protected ConfigurationService ConfigurationService => AppLocator.Current.GetRequiredService<ConfigurationService>();
        protected LocalizationService LocalizationService => AppLocator.Current.GetRequiredService<LocalizationService>();
        protected HomeAssistantService HomeAssistantService => AppLocator.Current.GetRequiredService<HomeAssistantService>();
        protected SchedulingService SchedulingService => AppLocator.Current.GetRequiredService<SchedulingService>();

        public int Index { get; set; }

        protected ConfigurationType configuration = new();
        public IConfigurationObject Configuration
        {
            get => configuration;
            set => configuration = (ConfigurationType)value;
        }

        public bool Enabled => configuration.Enabled;
        bool activated;

        protected abstract void ActivateIntegration();
        protected abstract void DeactivateIntegration();

        public void Activate()
        {
            if (Enabled && !activated)
            {
                ActivateIntegration();
                activated = true;
            }
        }
        public void Deactivate()
        {
            if (activated)
            {
                DeactivateIntegration();
                activated = false;
            }
        }
        public void Load()
        {
            var wasactivated = activated;
            if (activated)
            {
                Deactivate();
            }
            configuration = ConfigurationService.GetSavedConfigOrDefault<ConfigurationType>();
            if (wasactivated)
            {
                Activate();
            }
        }
        public void Save(IConfigurationObject configuration)
        {
            if (configuration is ConfigurationType config)
            {
                var wasactivated = activated;
                if (activated)
                {
                    Deactivate();
                }
                configuration = config;
                ConfigurationService.SaveConfig(config);
                if (wasactivated)
                {
                    Activate();
                }
            }
        }

        protected static void Register<I, VM, V>()
            where I : IIntegration, new()
            where VM : IIntegrationViewModel
            where V : Control, new()
        {
            AppLocator.CurrentMutable.RegisterLazySingleton<IIntegration>(() => new I());
            AppLocator.Current.GetRequiredService<IntegrationViewLocator>().Register(typeof(VM), () => new V());
        }

        public async Task Enable()
        {
            if (Enabled)
            {
                return;
            }
            configuration.Enabled = true;
            ConfigurationService.SaveConfig(configuration);
            await Task.Run(Activate);
        }

        public async Task Disable()
        {
            if (!Enabled)
            {
                return;
            }
            configuration.Enabled = false;
            ConfigurationService.SaveConfig(configuration);
            await Task.Run(Deactivate);
        }
    }
}
