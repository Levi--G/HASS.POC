using Avalonia.Utilities;
using HADesktop.Integrations;
using HADesktop.Integrations.Mouse;
using System.Collections.Generic;
using System.Linq;

namespace HADesktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [Reactive]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";

    [Reactive(nameof(ShowOverview), nameof(ShowConfig))]
    internal partial IIntegrationViewModel? SelectedIntegrationConfig { get; set; }
    public bool ShowOverview => SelectedIntegrationConfig == null;
    public bool ShowConfig => SelectedIntegrationConfig != null;
    [Reactive]
    internal partial List<IIntegration>? ActiveIntegrations { get; set; } = [];

    public MainViewModel()
    {
        //Test();
        LoadActiveIntegrations();
    }

    void LoadActiveIntegrations()
    {
        ActiveIntegrations = AppLocator.Current.GetServices<IIntegration>().Where(i => i.Configuration.Enabled).ToList();
    }

    [ReactiveCommand]
    private async void ConfigureIntegration(IIntegration integration)
    {
        SelectedIntegrationConfig = integration.ConfigurationViewModel;
        await SelectedIntegrationConfig.Load();
        SelectedIntegrationConfig.Activate();
    }

    [ReactiveCommand]
    private async void AddModule()
    {

    }

    [ReactiveCommand]
    private async void SaveModule()
    {
        if (SelectedIntegrationConfig != null)
        {
            await SelectedIntegrationConfig.Save();
        }
    }

    [ReactiveCommand]
    private async void UndoModule()
    {
        if (SelectedIntegrationConfig != null)
        {
            await SelectedIntegrationConfig.Load();
        }
    }

    [ReactiveCommand]
    private async void ReturnModule()
    {
        var vmb = SelectedIntegrationConfig;
        SelectedIntegrationConfig = null;
        vmb?.Dispose();
    }

    [ReactiveCommand]
    private async void DisableModule()
    {
        if (SelectedIntegrationConfig != null)
        {
            var vmb = SelectedIntegrationConfig;
            SelectedIntegrationConfig = null;
            var t = vmb.Disable();
            LoadActiveIntegrations();
            await t;
            vmb.Dispose();
        }
    }

    event Action? doupdate;
    async void Test()
    {
        await Task.Run(async () =>
        {
            await Task.Delay(2000).ConfigureAwait(false);
            var intgr = AppLocator.Current.GetService<IIntegration>();
            SelectedIntegrationConfig = intgr.ConfigurationViewModel;
            await SelectedIntegrationConfig.Load();
            SelectedIntegrationConfig.Activate();
            //doupdate += VMB.Update;
            Greeting = "1";
            //await Task.Delay(2000).ConfigureAwait(false);
            //VMB = new MouseConfigurationViewModel();
            //await VMB.Load();
            //VMB.Activate();
            //doupdate += VMB.Update;
            //Greeting = "2";
            //await Task.Delay(2000).ConfigureAwait(false);
            //VMB = new MouseConfigurationViewModel();
            //await VMB.Load();
            //VMB.Activate();
            //doupdate += VMB.Update;
            //Greeting = "3";
            //await Task.Delay(2000).ConfigureAwait(false);

            //doupdate?.Invoke();
        });
    }
}
