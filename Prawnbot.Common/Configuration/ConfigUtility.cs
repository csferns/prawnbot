using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Prawnbot.Common.Configuration
{
    public static class ConfigUtility
    {
        private static IConfigurationRoot GetConfigFile()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("Configuration\\appsettings.json", optional: false);

            return builder.Build();
        }

        private static string GetConfig(string parent, [CallerMemberName]string memberName = "")
        {
            string value = GetConfigFile().GetSection(parent).GetSection(memberName).Value;
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public static string TextFileDirectory => GetConfig("Generic") ?? Environment.CurrentDirectory + "\\Text Files";

        public static string CommandDelimiter => GetConfig("Generic") ?? "p!";

        public static bool AllowEventListeners => bool.Parse(GetConfig("Generic"));

        public static bool ProfanityFilter => bool.Parse(GetConfig("Generic"));

        public static bool DadMode => bool.Parse(GetConfig("Generic"));

        public static bool YottaMode => bool.Parse(GetConfig("Generic"));

        public static bool EmojiRepeat => bool.Parse(GetConfig("Generic"));

        public static bool AllowNonEssentialListeners => bool.Parse(GetConfig("Generic"));

        public static string MicrosoftSpeechServicesEndpoint => GetConfig("Endpoints");

        public static string MicrosoftSpeechServicesRegion => GetConfig("Keys");

        public static string GiphyEndpoint => GetConfig("Endpoints");

        public static string MicrosoftTranslateEndpoint => GetConfig("Endpoints");

        public static string R34Endpoint => GetConfig("Endpoints");

        public static string OverwatchStatsEndpoint => GetConfig("Endpoints");

        public static string ProfanityFilterEndpoint => GetConfig("Endpoints");

        /// <summary>
        /// Connection string for local database storage
        /// If UseDatabaseConnection is true, this is required.
        /// </summary>
        public static string DatabaseConnectionString => GetConfig("ConnectionStrings");

        public static string BotToken => GetConfig("Keys");

        public static string BlobStoreConnectionString => GetConfig("ConnectionStrings");

        public static string GiphyAPIKey => GetConfig("Keys");

        public static string TranslateAPIKey => GetConfig("Keys");

        public static string SpeechServicesKey => GetConfig("Keys");

        public static string GoogleAPIKey => GetConfig("Keys");

        public static string GoogleApplicationName => GetConfig("Keys");

        public static string GoogleClientSecret => GetConfig("Keys");
    }
}
