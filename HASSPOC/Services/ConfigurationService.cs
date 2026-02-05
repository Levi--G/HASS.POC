using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASSPOC.Services
{
    internal class ConfigurationService
    {
        public T GetSavedConfigOrDefault<T>() where T : IConfigurationObject, new()
        {
            //todo load from json
            var t = new T();
            t.Enabled = true; //demostration purposes
            return t;
        }
        public void StoreConfig<T>(T config) where T : IConfigurationObject, new()
        {
            //todo save in json
        }
        public void LoadConfigurationFromDisk()
        {
            //todo load
        }
    }
}
