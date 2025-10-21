using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Auxiliar.Helper
{
    public static class ConfigurationsManagerHelper
    {
        public static Dictionary<string, string> GetAppSettingsAsDictionary()
        {
            Dictionary<string, string> appSettings = new Dictionary<string, string>();
            NameValueCollection settings = System.Configuration.ConfigurationManager.AppSettings;

            foreach (string key in settings.Keys)
            {
                appSettings[key] = settings[key];
            }
            return appSettings;
        }
        public static Dictionary<string, string> GetAllAppSettings(
            Func<Dictionary<string, string>> getFrameworkSettings,
            Func<Dictionary<string, string>> getCoreSettings)
        {
            var allSettings = new Dictionary<string, string>();

            if (getFrameworkSettings != null)
            {
                var frameworkSettings = getFrameworkSettings();
                foreach (var kvp in frameworkSettings)
                {
                    allSettings[kvp.Key] = kvp.Value;
                }
            }

            if (getCoreSettings != null)
            {
                var coreSettings = getCoreSettings();
                foreach (var kvp in coreSettings)
                {
                    // Adiciona novas ou atualiza existentes (se quiser ter prioridade para o Core, por exemplo)
                    allSettings[kvp.Key] = kvp.Value;
                }
            }

            return allSettings;
        }
    }
}
