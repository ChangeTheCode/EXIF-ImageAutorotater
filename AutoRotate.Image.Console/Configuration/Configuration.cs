using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoRotate.Image.Console
{
    internal class Configuration : IConfigurationManager
    {

        /// <summary>
        /// set config, if key is not in file, create
        /// </summary>
        /// <param name="key">Nome do parâmetro</param>
        /// <param name="value">Valor do parâmetro</param>
        public void SetConfig(string key, string value)
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
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        /// <summary>
        /// Get key value, if not found, return null
        /// </summary>
        /// <param name="key"></param>
        /// <returns>null if key is not found, else string with value</returns>
        public string GetConfig(string key)
        {
            if(string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[key])){
                throw new ArgumentException("App setting values is not set of the key", key); 
            }
            return ConfigurationManager.AppSettings[key];
        }
    }
}
