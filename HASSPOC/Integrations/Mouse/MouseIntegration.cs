using System.Runtime.CompilerServices;

namespace HASSPOC.Integrations.Mouse;

internal class MouseIntegration : IntegrationBase<MouseConfiguration, MouseConfigurationViewModel>
{
    public static void Register()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            //example only supported on windows
            Register<MouseIntegration, MouseConfigurationViewModel, MouseConfigurationView>();
            //alternative:
            //AppLocator.CurrentMutable.RegisterLazySingleton<IIntegration>(() => new MouseIntegration());
            //AppLocator.Current.GetRequiredService<IntegrationViewLocator>().Register(typeof(MouseConfigurationViewModel), () => new MouseConfigurationView());
        }
    }

    protected override string LocalizationKey => "MouseIntegration";

    Sensor? X;
    Sensor? Y;
    ScheduledTask? updateTask;

    protected override void ActivateIntegration()
    {
        var ha = AppLocator.Current.GetRequiredService<HomeAssistantService>();
        X = ha.RegisterSensor();
        Y = ha.RegisterSensor();
        var sc = AppLocator.Current.GetRequiredService<SchedulingService>();
        updateTask = sc.Schedule(UpdateSensors, DateTime.Now, TimeSpan.FromSeconds(configuration.UpdateIntervals));
    }

    void UpdateSensors(CancellationToken token)
    {
        //get positions from api
        if (Win32Api.GetCursorPos(out var point))
        {
            X?.Value = point.X.ToString();
            Y?.Value = point.Y.ToString();
        }
        else
        {
            //log?
        }
    }

    protected override void DeactivateIntegration()
    {
        updateTask?.Cancel();
        updateTask = null;
        var ha = AppLocator.Current.GetRequiredService<HomeAssistantService>();
        ha.Unregister(X);
        ha.Unregister(Y);
        X = null;
        Y = null;
    }
}
