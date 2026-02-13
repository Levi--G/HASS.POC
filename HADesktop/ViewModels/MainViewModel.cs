using HADesktop.Integrations;
using System.Collections.Generic;
using System.Linq;
using Ursa.Controls;

namespace HADesktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [Reactive(nameof(ShowOverview), nameof(ShowConfig))]
    internal partial IIntegrationViewModel? SelectedIntegrationConfig { get; set; }
    public bool ShowOverview => SelectedIntegrationConfig == null;
    public bool ShowConfig => SelectedIntegrationConfig != null;
    [Reactive]
    internal partial List<IIntegration>? ActiveIntegrations { get; set; } = [];

    AppSettingsService AppSettingsService = AppLocator.Current.GetRequiredService<AppSettingsService>();

    public MainViewModel()
    {

    }

    public void Activate()
    {
        ShowWelcomeDialog();
        LoadActiveIntegrations();
    }

    void LoadActiveIntegrations()
    {
        ActiveIntegrations = AppLocator.Current.GetServices<IIntegration>().Where(i => i.Configuration.Enabled).ToList();
    }

    async void ShowWelcomeDialog()
    {
        if (AppSettingsService.Settings.IsOnboarding)
        {
            var result = await OverlayDialog.ShowCustomModal<WelcomeDialog, WelcomeDialogViewModel, object>(new WelcomeDialogViewModel(), options: new() { CanDragMove = false, IsCloseButtonVisible = false });
            if ((result as bool?) != true)
            {
                (App.Current as App)?.DoShutdown();
            }
        }
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
}
