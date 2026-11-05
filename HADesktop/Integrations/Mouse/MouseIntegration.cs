using HADesktop;
using HADesktop.Entities;
using HADesktop.Extensions;
using HADesktop.Integrations.Mouse;
using HADesktop.Services;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HADesktop.Integrations.Mouse;

internal class MouseIntegration : IntegrationBase<MouseConfiguration, MouseConfigurationViewModel>, IIntegration
{
    Sensor? X;
    Sensor? Y;
    Sensor? Activity;
    ScheduledTask? updateTask;
    Win32Api.LPPOINT? lastpoint;
    Win32Api.LPPOINT? lastlastpoint;

    protected override void ActivateIntegration()
    {
        var ha = AppLocator.Current.GetRequiredService<HomeAssistantService>();
        if (Config.EnableXY)
        {
            X = ha.RegisterSensor(Code, 0, "MouseX", unit_of_measurement: "px", icon: "mdi:cursor-default", suggested_display_precision: 0);
            Y = ha.RegisterSensor(Code, 0, "MouseY", unit_of_measurement: "px", icon: "mdi:cursor-default", suggested_display_precision: 0);
        }
        if (Config.EnableActivity)
        {
            Activity = ha.RegisterSensor(Code, 0, "MouseActivity", icon: "mdi:cursor-default");
        }
        var sc = AppLocator.Current.GetRequiredService<SchedulingService>();
        updateTask = sc.Schedule(UpdateSensors, DateTime.Now, TimeSpan.FromSeconds(Config.UpdateIntervals));
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
