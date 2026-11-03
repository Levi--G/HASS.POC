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

        protected AppSettingsService ConfigurationService => AppLocator.Current.GetRequiredService<AppSettingsService>();
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
        public void Reactivate()
        {
            if (activated)
            {
                Deactivate();
                Activate();
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
            ConfigurationService.ModifyConfig(() => configuration.Enabled = true);
            await Task.Run(Activate);
        }

        public async Task Disable()
        {
            if (!Enabled)
            {
                return;
            }
            ConfigurationService.ModifyConfig(() => configuration.Enabled = false);
            await Task.Run(Deactivate);
        }
    }
}
