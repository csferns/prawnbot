using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prawnbot.Common.Configuration
{
    public class ConfigUtility
    {
        private static IConfigurationRoot _configuration;

        #region Constructors
        public ConfigUtility()
        {

        }

        public ConfigUtility(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }
        #endregion

        #region Methods
        private static string GetConfig(string configurationName)
        {
            return _configuration.GetSection("ConfigValues").GetSection(configurationName).Value;
        }

        public static void SetConfig(string configurationName, string newConfigurationValue)
        {
            _configuration.GetSection("ConfigValues").GetSection(configurationName).Value = newConfigurationValue;
        }

        private static string GetConnectionString(string configurationName)
        {
            return _configuration.GetSection("ConnectionStrings").GetSection(configurationName).Value;
        }

        private static string GetLocalEnvSetting(string configurationName)
        {
            return _configuration.GetSection("LocalEnvSettings").GetSection(configurationName).Value;
        }

        public Dictionary<string, string> GetAllConfig()
        {
            Dictionary<string, string> configValues = new Dictionary<string, string>();

            foreach (var item in _configuration.GetSection("ConfigValues").GetChildren())
            {
                configValues.Add(item.Key, item.Value);
            }

            return configValues;
        }
        #endregion

        #region Generic App Config
        public string TextFileDirectory
        {
            get 
            {
                var config = GetLocalEnvSetting("TextFileDirectory");
                return !string.IsNullOrWhiteSpace(config) ? config : $"{Environment.CurrentDirectory}\\Text Files";
            }
        }

        public bool AllowEventListeners
        {
            get { return bool.Parse(GetConfig("AllowEventListeners")); }
            set { SetConfig("AllowEventListeners", value.ToString()); }
        }

        public bool ProfanityFilter
        {
            get { return bool.Parse(GetConfig("ProfanityFilter")); }
            set { SetConfig("ProfanityFilter", value.ToString()); }
        }

        public bool DadMode
        {
            get { return bool.Parse(GetConfig("DadMode")); }
        }

        public bool YottaMode
        {
            get { return bool.Parse(GetConfig("YottaMode")); }
        }
        #endregion

        #region Connection Strings
        public static string BotToken
        {
            get { return GetConnectionString("BotToken"); }
        }

        public string BlobStoreConnectionString
        {
            get { return GetConnectionString("BlobStoreConnectionString"); }
        }

        public string GiphyAPIKey
        {
            get { return GetConnectionString("GiphyAPIKey"); }
        }

        public string TranslateAPIKey
        {
            get { return GetConnectionString("TranslateAPIKey"); }
        }

        public string SpeechServicesKey
        {
            get { return GetConnectionString("SpeechServicesKey"); }
        }

        public static string DatabaseConnectionString
        {
            get { return GetConnectionString("DatabaseConnectionString"); }
        }

        public string GoogleAPIKey
        {
            get { return GetConnectionString("GoogleAPIKey"); }
        }

        public string GoogleApplicationName
        {
            get { return GetConnectionString("GoogleApplicationName"); }
        }
        #endregion
    }
}
