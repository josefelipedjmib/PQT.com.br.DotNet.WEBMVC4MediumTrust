using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;


namespace Auxiliar.Helper
{
    public static class ConfigurationManagerCoreHelper
    {

        private static string basePath = Path.Combine(AppContext.BaseDirectory, "");
        public static IConfiguration AppSetting { get; }

        static ConfigurationManagerCoreHelper() => AppSetting = new ConfigurationBuilder()
                                        .SetBasePath(basePath)
                                        .AddJsonFile("appsettings.json")
                                        .Build();

        public static Dictionary<string, string> GetAppSettingsAsDictionary(IConfiguration configuration)
        {
            Dictionary<string, string> appSettings = new Dictionary<string, string>();

            foreach (var section in configuration.GetChildren())
            {
                appSettings[section.Key] = section.Value;
            }
            return appSettings;
        }

    }
}
