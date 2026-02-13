using Irihi.Avalonia.Shared.Contracts;
using ReactiveUI;
using System.Threading.Tasks;
using Ursa.Controls;

namespace HADesktop.ViewModels;

public partial class WelcomeDialogViewModel : ViewModelBase, IDialogContext
{
    public event EventHandler<object?>? RequestClose;
    WindowNotificationManager? NotificationManager = AppLocator.Current.GetService<WindowNotificationManager>();
    LocalizationService LocalizationService => AppLocator.Current.GetRequiredService<LocalizationService>();
    AppSettingsService AppSettingsService => AppLocator.Current.GetRequiredService<AppSettingsService>();
    [Reactive] public partial string Language { get; set; }
    [Reactive] public partial string Host { get; set; }
    [Reactive] public partial bool UseTLS { get; set; }
    [Reactive] public partial bool UntrustedTLS { get; set; }
    [Reactive] public partial string User { get; set; }
    [Reactive] public partial string Password { get; set; }
    [Reactive] public partial string DeviceName { get; set; }
    [Reactive] public partial string SerialNumber { get; set; }
    [Reactive] public partial int SelectedLanguageIndex { get; set; }
    [Reactive] public partial bool IsTesting { get; set; }

    public WelcomeDialogViewModel()
    {
        Languages = LocalizationService.Languages.Select(l => l.Name).ToArray();
        SelectedLanguageIndex = Languages.IndexOf(LocalizationService.CurrentLanguage.Name);
        Host = AppSettingsService.Settings.Server;
        UseTLS = AppSettingsService.Settings.TLS;
        UntrustedTLS = AppSettingsService.Settings.AllowUntrustedTLS;
        User = AppSettingsService.Settings.Username;
        Password = AppSettingsService.Settings.Password;
        DeviceName = AppSettingsService.Settings.DeviceName;
        SerialNumber = AppSettingsService.Settings.SerialNumber;
        this.ObservableForProperty(m => m.SelectedLanguageIndex).Subscribe(i => LocalizationService.SetLanguage(LocalizationService.Languages[i.Value]));
    }

    public string[] Languages { get; set; }

    public void Close()
    {
        RequestClose?.Invoke(this, null);
    }

    [ReactiveCommand]
    private void Exit()
    {
        Close();
    }

    async Task<bool> TestConnection()
    {
        //todo
        IsTesting = true;
        await Task.Delay(2000);
        IsTesting = false;
        return false;
    }

    [ReactiveCommand]
    private async Task Test()
    {
        if (IsTesting)
        {
            return;
        }
        if (await TestConnection())
        {
            NotificationManager?.Show("Test Successful!", Avalonia.Controls.Notifications.NotificationType.Success);
        }
        else
        {
            NotificationManager?.Show("Test Failed, check the inputs.", Avalonia.Controls.Notifications.NotificationType.Error);
        }
    }

    [ReactiveCommand]
    private async Task Continue()
    {
        if (IsTesting)
        {
            return;
        }
        if (await TestConnection())
        {
            RequestClose?.Invoke(this, true);
        }
        else
        {
            NotificationManager?.Show("Test Failed, check the inputs.", Avalonia.Controls.Notifications.NotificationType.Error);
        }
    }
}