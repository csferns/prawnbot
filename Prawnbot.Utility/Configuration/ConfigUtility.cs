using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Prawnbot.Utility.Configuration
{
    public static class ConfigUtility
    {
        #region Methods
        private static IConfigurationRoot GetConfigFile()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("Configuration\\appsettings.json", optional: false);

            return builder.Build();
        }

        private static string GetConfig(string configurationName)
        {
            IConfigurationRoot configuration = GetConfigFile();
            return configuration.GetSection("ConfigValues").GetSection(configurationName).Value ?? string.Empty;

            // return CloudConfigurationManager.GetSetting(configurationName) ?? string.Empty;
        }

        public static string GetConnectionString(string connectionString)
        {
            IConfigurationRoot configuration = GetConfigFile();
            return configuration.GetSection("ConnectionStrings").GetSection(connectionString).Value ?? string.Empty;
        }

        public static string GetLocalEnvSetting(string configName)
        {
            IConfigurationRoot configuration = GetConfigFile();
            return configuration.GetSection("LocalEnvSettings").GetSection(configName).Value ?? string.Empty;
        }
        #endregion

        #region Generic App Config
        public static string TextFileDirectory
        {
            get
            {
                string configValue = GetLocalEnvSetting("TextFileDirectory");
                return string.IsNullOrEmpty(configValue) ? Environment.CurrentDirectory + "/Text Files" : configValue;
            }
        }

        public static string CommandDelimiter
        {
            get { return GetLocalEnvSetting("CommandDelimiter"); }
        }

        public static bool AllowEventListeners
        {
            get { return bool.Parse(GetConfig("AllowEventListeners")); }
        }

        public static bool ProfanityFilter
        {
            get { return bool.Parse(GetConfig("ProfanityFilter")); }
        }

        public static bool DadMode
        {
            get { return bool.Parse(GetConfig("DadMode")); }
        }

        public static bool YottaMode
        {
            get { return bool.Parse(GetConfig("YottaMode")); }
        }

        public static bool EmojiRepeat
        {
            get { return bool.Parse(GetConfig("EmojiRepeat")); }
        }
        #endregion

        #region Connection Strings
        public static string DatabaseConnectionString
        {
            get { return GetConnectionString("DatabaseConnectionString"); }
        }

        public static string BotToken
        {
            get { return GetConnectionString("BotToken"); }
        }

        public static string BlobStoreConnectionString
        {
            get { return GetConnectionString("BlobStoreConnectionString"); }
        }

        public static string GiphyAPIKey
        {
            get { return GetConnectionString("GiphyAPIKey"); }
        }

        public static string TranslateAPIKey
        {
            get { return GetConnectionString("TranslateAPIKey"); }
        }

        public static string SpeechServicesKey
        {
            get { return GetConnectionString("SpeechServicesKey"); }
        }

        public static string GoogleAPIKey
        {
            get { return GetConnectionString("GoogleAPIKey"); }
        }

        public static string GoogleApplicationName
        {
            get { return GetConnectionString("GoogleApplicationName"); }
        }
        #endregion
    }
}
