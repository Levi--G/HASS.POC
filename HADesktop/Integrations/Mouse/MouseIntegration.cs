using HADesktop;
using HADesktop.Entities;
using HADesktop.Extensions;
using HADesktop.Integrations.Mouse;
using HADesktop.Services;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HADesktop.Integrations.Mouse;

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

    protected override string LocalizationKey => "MouseModule";

    public override string Code => "MOU";

    Sensor? X;
    Sensor? Y;
    Sensor? Activity;
    ScheduledTask? updateTask;
    Win32Api.LPPOINT? lastpoint;
    Win32Api.LPPOINT? lastlastpoint;

    protected override void ActivateIntegration()
    {
        var ha = AppLocator.Current.GetRequiredService<HomeAssistantService>();
        if (configuration.EnableXY)
        {
            X = ha.RegisterSensor(Code, "MouseX", unit_of_measurement: "px", icon: "mdi:cursor-default", suggested_display_precision: 0);
            Y = ha.RegisterSensor(Code, "MouseY", unit_of_measurement: "px", icon: "mdi:cursor-default", suggested_display_precision: 0);
        }
        if (configuration.EnableActivity)
        {
            Activity = ha.RegisterSensor(Code, "MouseActivity", icon: "mdi:cursor-default");
        }
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
            if (Activity != null)
            {
                if (lastlastpoint != null && lastpoint != null)
                {
                    Activity.Value = (!lastpoint.Value.Equals(lastpoint.Value) || !lastpoint.Value.Equals(point)).ToString();
                }
                lastlastpoint = lastpoint;
                lastpoint = point;
            }
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
