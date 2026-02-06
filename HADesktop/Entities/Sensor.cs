using HADesktop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HADesktop.Entities
{
    internal class Sensor : IEntity
    {
        public string UniqueId => discovery.unique_id;

        public string? Value { get; set { if (value != field) { field = value; Publish(); } } }

        MQTTService MQTT;
        private readonly SensorDiscovery discovery;

        public Sensor(MQTTService mQTT, SensorDiscovery discovery)
        {
            MQTT = mQTT;
            this.discovery = discovery;
        }

        public void Publish()
        {
            MQTT.Publish(discovery.state_topic, Value);
        }

        public object GetDiscoveryComponent()
        {
            return discovery;
        }

        public record SensorDiscovery(string unique_id, string state_topic, DeviceClass? device_class, string? unit_of_measurement, bool? enabled_by_default,
            int? expire_after, bool? force_update, string? icon, string? name, string[]? options, int? suggested_display_precision, StateClass? state_class,
            string p = "sensor");
    }
}
