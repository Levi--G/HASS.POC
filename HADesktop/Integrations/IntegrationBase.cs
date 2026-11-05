using Avalonia.Controls;
using HADesktop.Extensions;
using HADesktop.Services;

namespace HADesktop.Integrations
{
    abstract internal class IntegrationBase<ConfigurationType, ViewModelType> : IIntegration
        where ConfigurationType : IConfigurationObject, new()
        where ViewModelType : IIntegrationViewModel, new()
    {
        public IIntegrationRegistration Registration { get; set; } = null!;
        protected string Code => Registration.Code;
        protected ConfigurationType Config = new();
        public IConfigurationObject Configuration
        {
            get => Config;
            set => Config = (ConfigurationType)value;
        }
        public bool Enabled => Config.Enabled;
        public int Index { get; set; }

        protected AppSettingsService ConfigurationService => AppLocator.Current.GetRequiredService<AppSettingsService>();
        protected HomeAssistantService HomeAssistantService => AppLocator.Current.GetRequiredService<HomeAssistantService>();
        protected SchedulingService SchedulingService => AppLocator.Current.GetRequiredService<SchedulingService>();

        private bool activated;

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

        public void Enable()
        {
            if (Enabled)
            {
                return;
            }
            ConfigurationService.ModifyConfig(() => Config.Enabled = true);
            Activate();
        }

        public void Disable()
        {
            if (!Enabled)
            {
                return;
            }
            ConfigurationService.ModifyConfig(() => Config.Enabled = false);
            Deactivate();
        }
    }
}
