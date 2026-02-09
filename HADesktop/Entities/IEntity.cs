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

    [JsonDerivedType(typeof(Sensor.SensorDiscovery))]
    record DiscoveryBase(string p);
}
