using DynamicData;
using HADesktop.Integrations;
using ReactiveUI;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HADesktop.Services;

internal class AppSettingsService
{
    public event Action? OnConfigStateChanged;

    public AppSettings Settings => settings;

    AppSettings settings = new();

    public string Version { get; private set; } = "0.0.0";
    public bool IsOnboarding { get => Settings.IsOnboarding; set { Settings.IsOnboarding = value; Save(); } }
    public bool IsConfigDirty
    {
        get; 
        private set
        {
            if (value != field)
            {
                field = value;
                OnConfigStateChanged?.Invoke();
            }
        }
    } = false;

    public AppSettingsService()
    {
    }

    public void Load()
    {
        LoadConfigurationFromDisk();
        if (settings.Enabled == false)
        {
            //first time loading, populate it with default values
            settings.Enabled = true;
            settings.DeviceName = Environment.MachineName;
            settings.SerialNumber = Guid.NewGuid().ToString();
            Save();
        }
        Version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? Version;
    }

    public void Verify()
    {
        using (configkey.Lock())
        {
            //remove parsing errors
            settings.ConfigurationObjects.RemoveAll(s => s.Config == null);
            //remove duplicate indexes
            var indexset = new HashSet<int>();
            settings.ConfigurationObjects.RemoveAll(s => !indexset.Add(s.Index));
        }
    }

    public void LoadIntegrations()
    {
        var registrations = AppLocator.Current.GetServices<IIntegrationRegistration>().ToList();
        using (configkey.Lock())
        {
            foreach (var conf in settings.ConfigurationObjects)
            {
                if (conf.Integration == null)
                {
                    var registration = registrations.FirstOrDefault(r => r.GetConfigType() == conf.Config.GetType());
                    if (registration != null)
                    {
                        var integration = registration.CreateInstance();
                        integration.Index = conf.Index;
                        integration.Configuration = conf.Config;
                        integration.Registration = registration;
                        conf.Integration = integration;
                    }
                }
            }
        }
    }

    public List<IIntegration> GetIntegrations()
    {
        using (configkey.Lock())
        {
            var list = new List<IIntegration>(settings.ConfigurationObjects.Count);
            foreach (var conf in settings.ConfigurationObjects)
            {
                if (conf.Integration != null)
                {
                    list.Add(conf.Integration);
                }
            }
            return list;
        }
    }

    public List<IIntegrationRegistration> GetPossibleIntegrationRegistrations()
    {
        using (configkey.Lock())
        {
            var list = new List<IIntegrationRegistration>();
            foreach (var reg in AppLocator.Current.GetServices<IIntegrationRegistration>())
            {
                if (!reg.SingleInstance || !settings.ConfigurationObjects.Any(s=>s.Config.GetType() == reg.GetConfigType()))
                {
                    list.Add(reg);
                }
            }
            return list;
        }
    }

    public IIntegration AddIntegration(IIntegrationRegistration registration)
    {
        var instance = registration.CreateInstance();
        using (configkey.Lock())
        {
            instance.Index = GetNextIndex();
            instance.Registration = registration;
            instance.Configuration = registration.GetDefaultConfig();
            settings.ConfigurationObjects.Add(new() { Config = instance.Configuration, Index = instance.Index, Integration = instance });
        }
        IsConfigDirty = true;
        return instance;
    }

    public void RemoveIntegration(IIntegration instance)
    {
        using (configkey.Lock())
        {
            instance.Disable();
            instance.Deactivate();
            settings.ConfigurationObjects.Remove(settings.ConfigurationObjects.First(c => c.Integration == instance));
        }
        IsConfigDirty = true;
    }

    /// <summary>
    /// Caller should lock configkey!
    /// </summary>
    /// <returns>The next available index</returns>
    int GetNextIndex()
    {
        int index = 0;
        while (settings.ConfigurationObjects.Any(c => c.Index == index))
        {
            index++;
        }
        return index;
    }

    public void Save()
    {
        SaveConfigurationToDisk();
    }
    SemaphoreSlim configkey = new(1, 1);

    /// <summary>
    /// Try not to use while app is running due to locking
    /// </summary>
    public void ReadConfig(Action mod)
    {
        using (configkey.Lock())
        {
            mod();
        }
    }
    public async Task ReadConfigAsync(Action mod)
    {
        using (await configkey.LockAsync())
        {
            mod();
        }
    }
    public T ReadConfig<T, C>(Func<C, T> mod, C config)
    {
        using (configkey.Lock())
        {
            return mod(config);
        }
    }
    public async Task<T> ReadConfigAsync<T, C>(Func<C, T> mod, C config)
    {
        using (await configkey.LockAsync())
        {
            return mod(config);
        }
    }
    /// <summary>
    /// Try not to use while app is running due to locking
    /// </summary>
    public void ModifyConfig(Action mod)
    {
        using (configkey.Lock())
        {
            mod();
        }
        IsConfigDirty = true;
    }
    public async Task ModifyConfigAsync(Action mod)
    {
        using (await configkey.LockAsync())
        {
            mod();
        }
        IsConfigDirty = true;
    }
    public void LoadConfigurationFromDisk()
    {
        var oldint = settings.ConfigurationObjects;
        try
        {
            if (File.Exists(jsonLocation))
            {
                settings = JsonSerializer.Deserialize(File.ReadAllText(jsonLocation), AppSettingsContext.Default.AppSettings) ?? settings;
            }
        }
        catch (Exception ex)
        {
            //TODO
        }
        if (oldint.Count > 0)
        {
            using (configkey.Lock())
            {
                foreach (var conf in oldint)
                {
                    if (conf.Integration != null)
                    {
                        var newstore = settings.ConfigurationObjects.FirstOrDefault(s => s.Index == conf.Index);
                        if (newstore != null && newstore.Config.GetType() == conf.Config.GetType())
                        {
                            conf.Integration.Configuration = newstore.Config;
                            newstore.Integration = conf.Integration;
                        }
                    }
                }
            }
        }
        IsConfigDirty = false;
    }
    public void SaveConfigurationToDisk()
    {
        try
        {
            if (File.Exists(jsonLocation))
            {
                var backupfile = Path.Combine(backupLocation, $"{DateTime.Now.ToString("yyyyMMdd HHmmss")}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(backupfile)!);
                File.Move(jsonLocation, backupfile);
            }
            Directory.CreateDirectory(Path.GetDirectoryName(jsonLocation)!);
            File.WriteAllText(jsonLocation, JsonSerializer.Serialize(settings, AppSettingsContext.Default.AppSettings));
        }
        catch (Exception ex)
        {
            //TODO
        }
        IsConfigDirty = false;
    }

    readonly string jsonLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.AppName, "config.json");
    readonly string backupLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.AppName, "backup");

    public class AppSettings
    {
        public bool Enabled { get; set; }
        public bool IsOnboarding { get; set; } = true;

        public string DeviceName { get; set; } = "";

        public string MQTTTopicBase { get; set; } = "HADesktop";

        public string SerialNumber { get; set; } = "";

        public string Server { get; set; } = Constants.DefaultHAHost;

        public string Username { get; set; } = Constants.AppName;

        public string Password { get; set; } = "";
        public int Port { get; set; } = Constants.DefaultPort;
        public bool TLS { get; set; } = Constants.DefaultTLS;
        public bool AllowUntrustedTLS { get; set; } = Constants.DefaultTLS;

        public string Language { get; set; } = "English";

        public List<IntegrationStore> ConfigurationObjects { get; set; } = [];
    }

    public class IntegrationStore
    {
        public int Index { get; set; }
        [JsonIgnore]
        public IIntegration? Integration { get; set; }
        public required IConfigurationObject Config { get; set; }
    }
}

[JsonSerializable(typeof(AppSettingsService.AppSettings))]
internal partial class AppSettingsContext : JsonSerializerContext
{

}