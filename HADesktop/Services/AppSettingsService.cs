using HADesktop.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HADesktop.Services
{
    internal class AppSettingsService
    {
        private ConfigurationService configurationService = AppLocator.Current.GetRequiredService<ConfigurationService>();

        public AppSettings Settings { get; private set; }

        public AppSettingsService()
        {
            Settings = configurationService.GetSavedConfigOrDefault<AppSettings>();
            if (Settings.Enabled == false)
            {
                //first time loading, populate it with default values
                Settings.Enabled = true;
                Settings.DeviceName = Environment.MachineName;
                Settings.Identifiers = new Guid().ToString();
                Save();
            }
        }

        public void Save()
        {
            configurationService.SaveConfig(Settings);
        }

        public class AppSettings : IConfigurationObject
        {
            public bool Enabled { get; set; }

            public string DeviceName { get; set; } = "";

            public string MQTTTopicBase { get; set; } = "HADesktop.Desktop";

            public string Identifiers { get; set; } = "";
        }
    }
}
