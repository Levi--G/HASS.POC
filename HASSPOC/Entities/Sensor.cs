using HASSPOC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASSPOC.Entities
{
    internal class Sensor : IEntity
    {
        public string? Value { get; set { field = value; Publish(); } }

        string topic;

        MQTTService MQTT;

        public Sensor(MQTTService mQTT, string topic)
        {
            MQTT = mQTT;
            this.topic = topic;
        }

        public void Publish()
        {
            MQTT.Publish(topic, Value);
        }

        public object GetDiscoveryComponent()
        {
            return new SensorDiscovery ("","","","");
        }

        public record SensorDiscovery(string device_class, string unit_of_measurement, string unique_id, string state_topic, string p = "sensor");
    }
}
