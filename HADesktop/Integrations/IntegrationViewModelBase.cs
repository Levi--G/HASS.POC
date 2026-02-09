using DynamicData.Binding;
using HADesktop.Integrations;
using HADesktop.Services;
using HADesktop.ViewModels;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;

namespace HADesktop.Integrations
{
    internal abstract partial class IntegrationViewModelBase<ConfigurationType> : ViewModelBase, IIntegrationViewModel
        where ConfigurationType : class, IConfigurationObject, new()
    {
        public IIntegration? Integration { get; init; }
        public IConfigurationObject Configuration { get; init; } = new ConfigurationType();

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

        public async Task Load()
        {
            if (IsLoading)
            {
                return;
            }
            IsLoading = true;
            using (DelayChangeNotifications())
            {
                await Task.Run(async () => await SetConfig(Configuration as ConfigurationType ?? new ConfigurationType()));
            }
            IsDirty = false;
            IsLoading = false;
        }

        public async Task Save()
        {
            if(IsLoading)
            {
                return;
            }
            IsLoading = true;
            IsSaving = true;
            await Task.Run(async () =>
            {
                Integration?.Save(await GetConfig());
            });
            IsDirty = false;
            IsLoading = false;
            IsSaving = false;
        }

        protected abstract ValueTask SetConfig(ConfigurationType config);
        protected abstract ValueTask<ConfigurationType> GetConfig();
        protected abstract void Activate(CompositeDisposable disposables);

        public void Dispose()
        {
            disposables.Dispose();
        }

        public Task Enable()
        {
            if (Integration != null)
            {
                return Integration.Enable();
            }
            return Task.CompletedTask;
        }

        public Task Disable()
        {
            if (Integration != null)
            {
                return Integration.Disable();
            }
            return Task.CompletedTask;
        }
    }
}
