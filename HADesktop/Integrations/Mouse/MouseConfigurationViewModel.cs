using DynamicData.Binding;
using HADesktop.Integrations.Mouse;
using System.Reactive.Disposables;

namespace HADesktop.Integrations.Mouse;

internal partial class MouseConfigurationViewModel : IntegrationViewModelBase<MouseConfiguration>
{
    [Reactive]
    public partial int UpdateIntervals { get; set; }

    public void Update()
    {
        UpdateIntervals += 1;
    }

    protected override void Activate(CompositeDisposable disposables)
    {
        this.WhenAnyPropertyChanged(nameof(UpdateIntervals)).Subscribe(s => IsDirty = true);
    }

    protected override ValueTask<MouseConfiguration> GetConfig()
    {
        return ValueTask.FromResult(new MouseConfiguration() { UpdateIntervals = UpdateIntervals });
    }

    protected override async ValueTask SetConfig(MouseConfiguration config)
    {
        await Task.Delay(1000);
        UpdateIntervals = config.UpdateIntervals;
    }
}
