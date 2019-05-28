using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Prawnbot.Core.Utility
{
    public class ConfigUtility
    {
        private IConfigurationRoot _configuration;

        public ConfigUtility()
        {

        }

        public ConfigUtility(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        private string GetConfig(string configurationName)
        {
            return _configuration.GetSection("ConfigValues").GetSection(configurationName).Value;
        }

        private string GetConnectionString(string configurationName)
        {
            return _configuration.GetSection("ConnectionStrings").GetSection(configurationName).Value;
        }

        private string GetLocalEnvSetting(string configurationName)
        {
            return _configuration.GetSection("LocalEnvSettings").GetSection(configurationName).Value;
        }

        public void SetConfig(string configurationName, string newConfigurationValue)
        {
            _configuration.GetSection("ConfigValues").GetSection(configurationName).Value = newConfigurationValue;
        }

        public Dictionary<string, string> GetAllConfig()
        {
            Dictionary<string, string> configValues = new Dictionary<string, string>();

            foreach (var item in _configuration.GetSection("ConfigValues").AsEnumerable())
            {
                configValues.Add(item.Key, item.Value);
            }

            return configValues;
        }

        public bool AllowEventListeners
        {
            get { return bool.Parse(GetConfig("AllowEventListeners"));  }
        }

        public string BlobStoreConnectionString
        {
            get { return GetConnectionString("BlobStoreConnectionString"); }
        }

        public string TextFileDirectory
        {
            get 
            {
                var config = GetLocalEnvSetting("TextFileDirectory");
                return !string.IsNullOrWhiteSpace(config) ? config : $"{Environment.CurrentDirectory}\\Text Files";
            }
        }

        public bool ProfanityFilter
        {
            get { return bool.Parse(GetConfig("ProfanityFilter")); }
        }

        public string GiphyAPIKey
        {
            get { return GetConnectionString("GiphyAPIKey"); }
        }

        public string TranslateAPIKey
        {
            get { return GetConnectionString("TranslateAPIKey"); }
        }
    }
}
