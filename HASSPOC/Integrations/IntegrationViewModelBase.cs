using DynamicData.Binding;
using HASSPOC.ViewModels;

namespace HASSPOC.Integrations
{
    internal abstract partial class IntegrationViewModelBase<ConfigurationType> : ViewModelBase, IIntegrationViewModel
        where ConfigurationType : class, IConfigurationObject, new()
    {
        public IIntegration? Integration { get; init; }
        public IConfigurationObject? Configuration { get; init; }

        [Reactive]
        public partial bool IsDirty { get; private set; } = true;

        public IntegrationViewModelBase()
        {
            this.WhenAnyPropertyChanged().Subscribe((a) => IsDirty = true);
        }

        public void Activate()
        {
            Revert();
        }

        public void Revert()
        {
            SetConfig(Configuration as ConfigurationType ?? new ConfigurationType());
            IsDirty = false;
        }

        public void Save()
        {
            Integration?.Save(GetConfig());
            IsDirty = false;
        }

        protected abstract void SetConfig(ConfigurationType config);
        protected abstract ConfigurationType GetConfig();
    }
}
