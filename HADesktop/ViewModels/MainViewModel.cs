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
    [Reactive]
    internal partial bool IsConfigurationDirty { get; set; } = false;

    AppSettingsService AppSettingsService = AppLocator.Current.GetRequiredService<AppSettingsService>();

    public MainViewModel()
    {

    }

    public void Activate()
    {
        ShowWelcomeDialog();
        LoadActiveIntegrations();
        AppSettingsService.OnConfigStateChanged += AppSettingsService_OnConfigStateChanged;
        AppSettingsService_OnConfigStateChanged();
    }

    private void AppSettingsService_OnConfigStateChanged()
    {
        IsConfigurationDirty = AppSettingsService.IsConfigDirty;
    }

    public void Deactivate()
    {
        AppSettingsService.OnConfigStateChanged -= AppSettingsService_OnConfigStateChanged;
    }

    void LoadActiveIntegrations()
    {
        ActiveIntegrations = AppLocator.Current.GetRequiredService<AppSettingsService>().GetIntegrations();
    }

    async void ShowWelcomeDialog()
    {
        if (AppSettingsService.Settings.IsOnboarding)
        {
            var result = await OverlayDialog.ShowCustomAsync<WelcomeDialog, WelcomeDialogViewModel, object>(new WelcomeDialogViewModel(), options: new() { CanDragMove = false, IsCloseButtonVisible = false });
            if ((result as bool?) != true)
            {
                (App.Current as App)?.DoShutdown();
            }
            else
            {
                AppSettingsService.Settings.IsOnboarding = false;
                AppSettingsService.Save();
                await AppLocator.Current.GetRequiredService<MQTTService>().StartConnect();
            }
        }
    }

    [ReactiveCommand]
    private async Task ConfigureIntegration(IIntegration integration)
    {
        SelectedIntegrationConfig = integration.Registration.CreateViewModel(integration);
        await SelectedIntegrationConfig.Discard();
        SelectedIntegrationConfig.Activate();
    }

    [ReactiveCommand]
    private async Task ToggleIntegration(IIntegration integration)
    {
        if (integration.Enabled)
        {
            integration.Disable();
        }
        else
        {
            integration.Enable();
        }
    }

    [ReactiveCommand]
    private async Task AddModule()
    {
        var result = await OverlayDialog.ShowCustomAsync<ModuleDialog, ModuleDialogViewModel, IIntegrationRegistration>(new ModuleDialogViewModel(), options: new() { CanDragMove = false });
        if (result != null)
        {
            AppSettingsService.AddIntegration(result);
            LoadActiveIntegrations();
        }
    }

    [ReactiveCommand]
    private async Task ApplyModule()
    {
        if (SelectedIntegrationConfig != null)
        {
            await SelectedIntegrationConfig.Apply();
        }
    }

    [ReactiveCommand]
    private async Task DiscardModule()
    {
        if (SelectedIntegrationConfig != null)
        {
            await SelectedIntegrationConfig.Discard();
        }
    }

    [ReactiveCommand]
    private async Task ReturnModule()
    {
        var vmb = SelectedIntegrationConfig;
        SelectedIntegrationConfig = null;
        vmb?.Dispose();
    }

    [ReactiveCommand]
    private async Task RemoveModule()
    {
        if (SelectedIntegrationConfig != null)
        {
            var vmb = SelectedIntegrationConfig;
            SelectedIntegrationConfig = null;
            AppSettingsService.RemoveIntegration(vmb.Integration);
            LoadActiveIntegrations();
            vmb.Dispose();
        }
    }

    [ReactiveCommand]
    private async Task SaveConfiguration()
    {
        AppSettingsService.SaveConfigurationToDisk();
    }
}
