using HASS.Entities;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Services
{
    internal class HomeAssistantService
    {
        MQTTService MQTT = AppLocator.Current.GetRequiredService<MQTTService>();
        AppSettingsService AppSettingsService = AppLocator.Current.GetRequiredService<AppSettingsService>();

        public HomeAssistantService()
        {

        }

        List<IEntity> entities = new();

        public Sensor RegisterSensor(string modulecode, string name, string unique_id, DeviceClass? device_class, string? unit_of_measurement, bool? enabled_by_default,
            int? expire_after, bool? force_update, string? icon, string[]? options, int? suggested_display_precision, StateClass? state_class)
        {
            var uni = GetUniqueId(modulecode, name);
            var topic = GetTopic(uni);
            var s = new Sensor(MQTT, new(uni, topic, device_class, unit_of_measurement, enabled_by_default, expire_after, force_update, icon, name, options, suggested_display_precision, state_class));
            entities.Add(s);
            return s;
        }

        public void Unregister(IEntity? entity)
        {
            if (entity != null)
            {
                entities.Remove(entity);
            }
        }

        public void SendDiscovery()
        {
            // combine the entities discoveries and send update to device/config topic
        }

        string GetTopic(string uniqueId)
        {
            return $"{AppSettingsService.Settings.MQTTTopicBase}/{AppSettingsService.Settings.DeviceName}/{uniqueId}";
        }

        string GetUniqueId(string modulecode, string name)
        {
            var id = new string((Constants.UniqueIdPrefix + modulecode + name).Replace(' ', '_').Replace('-', '_').Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
            if (entities.Any(e => e.UniqueId == id))
            {
                int addition = 0;
                while (entities.Any(e => e.UniqueId == id + addition))
                {
                    addition++;
                }
                id += addition;
            }
            return id;
        }
    }
}
