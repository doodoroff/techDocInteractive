using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TechDocInteractive
{
    class AppSettings
    {
        public static string GetCurrentFilePath(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;

            return appSettings[key] ?? "Not Found";
        }

        public static void SetCurrentFilePath(string key, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
            configFile.Save();
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
    }
}
