using HASSPOC.Integrations.Mouse;
using System.Text.Json.Serialization;

namespace HASSPOC.Services
{
    /// <summary>
    /// Used for objects to be stored by the ConfigurationService
    /// Add the implementation to the interface as a derived type
    /// </summary>
    [JsonDerivedType(typeof(MouseConfiguration), "mouse")]
    
    public interface IConfigurationObject
    {
        public bool Enabled { get; set; }
    }
}
