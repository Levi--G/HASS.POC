using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HADesktop.Entities
{
    internal interface IEntity
    {
        string UniqueId { get; }
        string Name { get; }

        DiscoveryBase GetDiscoveryComponent();
    }
    internal interface IPublishEntity : IEntity
    {
        event Action<IEntity>? OnRequestPublish;

        void Publish(MQTTService MQTT);
        void RequestPublish();
    }
    internal interface ISubscribeEntity : IEntity
    {
        event Action<IEntity>? OnStateChange;

        void Subscribe(MQTTService MQTT);
        void RequestPublish();
    }
    internal interface ILastWillEntity : IEntity
    {
        void SetLastWill(MQTTService MQTT);
    }
    internal interface IAvailabilityEntity : ILastWillEntity
    {
        event Action<IEntity>? OnRequestPublishAvailability;
        bool Available { get; set; }

        void PublishAvailability(MQTTService MQTT);
        void RequestPublishAvailability();
    }

    [JsonDerivedType(typeof(Sensor.SensorDiscovery))]
    record DiscoveryBase(string p);
}
