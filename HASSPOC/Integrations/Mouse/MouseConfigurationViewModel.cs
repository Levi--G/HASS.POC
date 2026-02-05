namespace HASSPOC.Integrations.Mouse;

internal partial class MouseConfigurationViewModel : IntegrationViewModelBase<MouseConfiguration>
{
    [Reactive]
    public partial int UpdateIntervals { get; set; }

    protected override MouseConfiguration GetConfig()
    {
        return new MouseConfiguration() { UpdateIntervals = UpdateIntervals };
    }

    protected override void SetConfig(MouseConfiguration config)
    {
        UpdateIntervals = config.UpdateIntervals;
    }
}
