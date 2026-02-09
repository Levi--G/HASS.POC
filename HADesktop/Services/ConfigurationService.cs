using DynamicData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace HADesktop.Services
{
    internal class ConfigurationService
    {
        SemaphoreSlim configkey = new(1, 1);

        /// <summary>
        /// Try not to use while app is running due to locking
        /// </summary>
        public T GetSavedConfigOrDefault<T>() where T : IConfigurationObject, new()
        {
            using (configkey.Lock())
            {
                return configurationObjects.OfType<T>().FirstOrDefault() ?? new();
            }
        }
        public async Task<T> GetSavedConfigOrDefaultAsync<T>() where T : IConfigurationObject, new()
        {
            using (await configkey.LockAsync())
            {
                return configurationObjects.OfType<T>().FirstOrDefault() ?? new();
            }
        }
        /// <summary>
        /// Try not to use while app is running due to locking
        /// </summary>
        public void SaveConfig<T>(T? config) where T : IConfigurationObject, new()
        {
            using (configkey.Lock())
            {
                SaveConfigInternal(config);
            }
        }
        public async Task SaveConfigAsync<T>(T? config) where T : IConfigurationObject, new()
        {
            using (await configkey.LockAsync())
            {
                await Task.Run(() => SaveConfigInternal(config));
            }
        }
        public void SaveConfigInternal<T>(T? config) where T : IConfigurationObject, new()
        {
            var existing = configurationObjects.FindIndex(c => c is T);
            if (config != null)
            {
                if (existing != -1)
                {
                    configurationObjects[existing] = config;
                }
                else
                {
                    configurationObjects.Add(config);
                }
            }
            else
            {
                if (existing != -1)
                {
                    configurationObjects.RemoveAt(existing);
                }
            }
            SaveConfigurationToDisk();
        }
        public void LoadConfigurationFromDisk()
        {
            try
            {
                if (File.Exists(jsonLocation))
                {
                    configurationObjects = JsonSerializer.Deserialize<List<IConfigurationObject>>(File.ReadAllText(jsonLocation), jsonOptions) ?? configurationObjects;
                }
            }
            catch (Exception ex)
            {
                //TODO
            }
        }
        public void SaveConfigurationToDisk()
        {
            try
            {
                if (File.Exists(jsonLocation))
                {
                    //TODO write backup
                    var backupfile = Path.Combine(backupLocation, $"{DateTime.Now.ToString("yyyyMMdd HHmmss")}.json");
                    Directory.CreateDirectory(Path.GetDirectoryName(backupfile)!);
                    File.Move(jsonLocation, backupfile);
                }
                Directory.CreateDirectory(Path.GetDirectoryName(jsonLocation)!);
                File.WriteAllText(jsonLocation, JsonSerializer.Serialize<List<IConfigurationObject>>(configurationObjects, jsonOptions));
            }
            catch (Exception ex)
            {
                //TODO
            }
        }

        JsonSerializerOptions jsonOptions;

        List<IConfigurationObject> configurationObjects = [];

        readonly string jsonLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.AppName, "config.json");
        readonly string backupLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.AppName, "backup");

        public ConfigurationService()
        {
            var typeInfo = new DefaultJsonTypeInfoResolver().WithAddedModifier(AddConfigurationObjectModifier);
            jsonOptions = new JsonSerializerOptions() { TypeInfoResolver = typeInfo };
            LoadConfigurationFromDisk();
        }

        void AddConfigurationObjectModifier(JsonTypeInfo info)
        {
            if (info.Type == typeof(IConfigurationObject))
            {
                info.PolymorphismOptions = new();
                var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass && typeof(IConfigurationObject).IsAssignableFrom(t)).Select(t => new JsonDerivedType(t, t.Name.Replace("Configuration", "")));
                info.PolymorphismOptions.DerivedTypes.AddRange(types);
            }
        }
    }
}
