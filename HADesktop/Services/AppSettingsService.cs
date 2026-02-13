using System.Reflection;

namespace HADesktop.Services
{
    internal class AppSettingsService
    {
        private ConfigurationService configurationService = AppLocator.Current.GetRequiredService<ConfigurationService>();

        public AppSettings Settings => settings ?? throw new Exception("Application was not loaded before accessing settings");

        AppSettings? settings;

        public string Version { get; private set; } = "0.0.0";
        public bool IsOnboarding { get => Settings.IsOnboarding; set { Settings.IsOnboarding = value; Save(); } }

        public AppSettingsService()
        {
        }

        public void Load()
        {
            settings = configurationService.GetSavedConfigOrDefault<AppSettings>();
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

        public void Save()
        {
            configurationService.SaveConfig(Settings);
        }

        public class AppSettings : IConfigurationObject
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
        }
    }
}
