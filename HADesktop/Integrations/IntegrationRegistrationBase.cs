using Avalonia.Controls;
using HADesktop.Extensions;
using HADesktop.Services;
using ReactiveUI.Primitives.Advanced;

namespace HADesktop.Integrations
{
    abstract internal class IntegrationRegistrationBase<IntegrationRegistrationType, IntegrationType, ConfigurationType, ViewModelType, ViewType> : IIntegrationRegistration
        where IntegrationRegistrationType : IIntegrationRegistration, new()
        where IntegrationType : IIntegration, new()
        where ConfigurationType : IConfigurationObject, new()
        where ViewModelType : IIntegrationViewModel, new()
        where ViewType : Control, new()
    {
        public string Name => LocalizationService.GetLocalizedByKey(Code + ".Name");
        public string Description => LocalizationService.GetLocalizedByKey(Code + ".Description");
        public abstract string Code { get; }

        public abstract bool SingleInstance { get; }

        protected LocalizationService LocalizationService => AppLocator.Current.GetRequiredService<LocalizationService>();

        public virtual IIntegration CreateInstance()
        {
            return new IntegrationType();
        }

        public Type GetConfigType()
        {
            return typeof(ConfigurationType);
        }

        public virtual IConfigurationObject GetDefaultConfig()
        {
            return new ConfigurationType();
        }

        public virtual IIntegrationViewModel CreateViewModel(IIntegration integration)
        {
            return new ViewModelType() { Integration = integration };
        }

        protected static void RegisterDefaults()
        {
            AppLocator.CurrentMutable.RegisterLazySingleton<IIntegrationRegistration>(() => new IntegrationRegistrationType());
            AppLocator.Current.GetRequiredService<IntegrationViewLocator>().Register(typeof(ViewModelType), () => new ViewType());
        }
    }
}
