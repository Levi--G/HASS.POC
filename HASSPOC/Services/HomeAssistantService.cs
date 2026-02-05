using HASSPOC.Entities;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASSPOC.Services
{
    internal class HomeAssistantService
    {
        MQTTService MQTT;

        public HomeAssistantService()
        {
            MQTT = AppLocator.Current.GetRequiredService<MQTTService>();
        }

        List<IEntity> entities = new();

        public Sensor RegisterSensor()
        {
            var s = new Sensor(MQTT, "");
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
    }
}
