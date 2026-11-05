using HADesktop.ViewModels;
using System.Reactive.Disposables;

namespace HADesktop.Integrations
{
    internal abstract partial class IntegrationViewModelBase<ConfigurationType> : ViewModelBase, IIntegrationViewModel
        where ConfigurationType : class, IConfigurationObject, new()
    {
        public IIntegration Integration { get; set; } = null!;
        public IConfigurationObject Configuration => Integration.Configuration;
        public ConfigurationType Config => (ConfigurationType)Configuration;

        [Reactive]
        public partial bool IsDirty { get; protected set; } = false;

        [Reactive]
        public partial bool IsLoading { get; private set; } = false;

        [Reactive]
        public partial bool IsSaving { get; private set; } = false;

        [Reactive]
        public partial bool IsConfigValid { get; protected set; } = true;

        CompositeDisposable disposables = new();

        public IntegrationViewModelBase()
        {

        }

        public void Activate()
        {
            Activate(disposables);
        }

        public async Task Discard()
        {
            if (IsLoading)
            {
                return;
            }
            IsLoading = true;
            using (DelayChangeNotifications())
            {
                await LoadConfig();
            }
            IsDirty = false;
            IsLoading = false;
        }

        public async Task Apply()
        {
            if (IsLoading)
            {
                return;
            }
            IsLoading = true;
            IsSaving = true;
            await ApplyConfig();
            IsDirty = false;
            IsLoading = false;
            IsSaving = false;
        }

        protected abstract ValueTask LoadConfig();
        protected abstract ValueTask ApplyConfig();
        protected abstract void Activate(CompositeDisposable disposables);

        public void Dispose()
        {
            disposables.Dispose();
        }

        public Task Enable()
        {
            if (Integration != null)
            {
                Integration.Enable();
            }
            return Task.CompletedTask;
        }

        public Task Disable()
        {
            if (Integration != null)
            {
                Integration.Disable();
            }
            return Task.CompletedTask;
        }
    }
}
