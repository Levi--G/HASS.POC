using DynamicData.Binding;
using System.Reactive.Disposables;

namespace HADesktop.Integrations.Mouse;

internal partial class MouseConfigurationViewModel : IntegrationViewModelBase<MouseConfiguration>
{
    [Reactive]
    public partial int UpdateIntervals { get; set; }

    protected override void Activate(CompositeDisposable disposables)
    {
        disposables.Add(this.WhenAnyPropertyChanged(nameof(UpdateIntervals)).Subscribe(s => IsDirty = true));
    }

    protected override ValueTask ApplyConfig()
    {
        Config.UpdateIntervals = UpdateIntervals;
        return ValueTask.CompletedTask;
    }

    protected override ValueTask LoadConfig()
    {
        UpdateIntervals = Config.UpdateIntervals;
        return ValueTask.CompletedTask;
    }
}
