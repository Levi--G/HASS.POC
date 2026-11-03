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
    public AppSettings Settings => settings;

    AppSettings settings = new();

    public string Version { get; private set; } = "0.0.0";
    public bool IsOnboarding { get => Settings.IsOnboarding; set { Settings.IsOnboarding = value; Save(); } }

    public AppSettingsService()
    {
        LoadConfigurationFromDisk();
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
            settings.configurationObjects.RemoveAll(s => s.Config == null);
            //remove duplicate indexes
            var indexset = new HashSet<int>();
            settings.configurationObjects.RemoveAll(s => !indexset.Add(s.Index));
        }
    }

    public void LoadIntegrations(List<IIntegrationRegistration> registrations)
    {
        using (configkey.Lock())
        {
            foreach (var conf in settings.configurationObjects)
            {
                if (conf.Integration == null)
                {
                    var registration = registrations.FirstOrDefault(r => r.GetConfigType() == conf.Config.GetType());
                    if (registration != null)
                    {
                        var integration = registration.CreateInstance();
                        integration.Index = conf.Index;
                        integration.Configuration = conf.Config;
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
            var list = new List<IIntegration>(settings.configurationObjects.Count);
            foreach (var conf in settings.configurationObjects)
            {
                if (conf.Integration != null)
                {
                    list.Add(conf.Integration);
                }
            }
            return list;
        }
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
    }
    public async Task ModifyConfigAsync(Action mod)
    {
        using (await configkey.LockAsync())
        {
            mod();
        }
    }
    public void LoadConfigurationFromDisk()
    {
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

        public List<IntegrationStore> configurationObjects = [];
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