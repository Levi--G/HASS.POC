using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HADesktop.Services
{
    internal class LocalizationService
    {
        Language fallbackLanguage;
        Language preferenceLanguage;
        AppSettingsService appSettingsService = AppLocator.Current.GetRequiredService<AppSettingsService>();

        public event Action? LanguageChanged;

        public LocalizationService()
        {
            Languages = [new("English", new("en-US")), new("Nederlands", new("nl-NL")), new("Français", new("fr-FR"))];
            fallbackLanguage = Languages.First();
            if (appSettingsService.IsOnboarding)
            {
                var ui = CultureInfo.CurrentUICulture;
                preferenceLanguage = Languages.FirstOrDefault(l => l.CultureInfo.TwoLetterISOLanguageName == ui.TwoLetterISOLanguageName) ?? fallbackLanguage;
                SetLanguage(preferenceLanguage);
            }
            else
            {
                preferenceLanguage = Languages.FirstOrDefault(l => l.Name == appSettingsService.Settings.Language) ?? fallbackLanguage;
            }
        }

        public Language[] Languages { get; }

        public Language CurrentLanguage => preferenceLanguage;

        public void SetLanguage(Language language)
        {
            appSettingsService.Settings.Language = language.Name;
            appSettingsService.Save();
            preferenceLanguage = language;
            LanguageChanged?.Invoke();
        }

        public string GetLocalizedByKey(string key)
        {
            return Localization.Localization.ResourceManager.GetString(key, preferenceLanguage.CultureInfo) ?? Localization.Localization.ResourceManager.GetString(key, fallbackLanguage.CultureInfo) ?? key;
        }

        public record Language(string Name, CultureInfo CultureInfo);
    }
}
