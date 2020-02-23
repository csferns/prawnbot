using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Prawnbot.Common.Configuration
{
    public static class ConfigUtility
    {
        private static IConfigurationRoot Config;

        static ConfigUtility()
        {
            Config = GetConfigFile();
        }

        private static IConfigurationRoot GetConfigFile()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("Configuration\\appsettings.json", optional: false);

            return builder.Build();
        }

        private static string GetConfig(string parent, [CallerMemberName]string memberName = "")
        {
            string value = Config.GetSection(parent).GetSection(memberName).Value;
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public static string TextFileDirectory => GetConfig("Generic") ?? Environment.CurrentDirectory + "\\Text Files";

        public static string CommandDelimiter => GetConfig("Generic") ?? "p!";

        public static bool AllowEventListeners => bool.Parse(GetConfig("EventListeners"));

        public static bool ProfanityFilter => bool.Parse(GetConfig("EventListeners"));

        public static bool DadMode => bool.Parse(GetConfig("EventListeners"));

        public static bool YottaMode => bool.Parse(GetConfig("EventListeners"));

        public static bool EmojiRepeat => bool.Parse(GetConfig("EventListeners"));

        public static bool UserJoined => bool.Parse(GetConfig("EventListeners"));

        public static bool UserBanned => bool.Parse(GetConfig("EventListeners"));

        public static bool UserUnbanned => bool.Parse(GetConfig("EventListeners"));

        public static bool MessageDeleted => bool.Parse(GetConfig("EventListeners"));

        public static bool JoinedGuild => bool.Parse(GetConfig("EventListeners"));

        public static string MicrosoftSpeechServicesEndpoint => GetConfig("Endpoints");

        public static string MicrosoftSpeechServicesRegion => GetConfig("Keys");

        public static string GiphyEndpoint => GetConfig("Endpoints");

        public static string MicrosoftTranslateEndpoint => GetConfig("Endpoints");

        public static string R34Endpoint => GetConfig("Endpoints");

        /// <summary>
        /// Endpoint to be used when calling off to the Overwatch stats API
        /// </summary>
        public static string OverwatchStatsEndpoint => GetConfig("Endpoints");

        public static string ProfanityFilterEndpoint => GetConfig("Endpoints");

        /// <summary>
        /// If the database connection is intitialised
        /// </summary>
        public static bool UseDatabaseConnection => bool.Parse(GetConfig("Generic"));

        /// <summary>
        /// Connection string for local database storage
        /// If UseDatabaseConnection is true, this is required.
        /// </summary>
        public static string DatabaseConnectionString => GetConfig("ConnectionStrings");

        /// <summary>
        /// The token to be used when connecting the bot to the Discord API
        /// </summary>
        public static string BotToken => GetConfig("Keys");

        public static string BlobStoreConnectionString => GetConfig("ConnectionStrings");

        public static string GiphyAPIKey => GetConfig("Keys");

        public static string TranslateAPIKey => GetConfig("Keys");

        public static string MicrosoftSpeechServicesKey => GetConfig("Keys");

        public static string GoogleAPIKey => GetConfig("Keys");

        public static string GoogleApplicationName => GetConfig("Keys");

        public static string GoogleClientSecret => GetConfig("Keys");

        /// <summary>
        /// Gets the text colour of the Console
        /// </summary>
        public static ConsoleColor ConsoleForeground => Enum.Parse<ConsoleColor>(GetConfig("Generic"));

        /// <summary>
        /// Gets the background colour of the Console
        /// </summary>
        public static ConsoleColor ConsoleBackground => Enum.Parse<ConsoleColor>(GetConfig("Generic"));

        /// <summary>
        /// Gets the Title of the Console
        /// </summary>
        public static string ConsoleTitle => GetConfig("Generic");

        /// <summary>
        /// Gets the encoding to be used when displaying text in the Console
        /// </summary>
        public static Encoding ConsoleEncoding => Encoding.GetEncoding(GetConfig("Generic"));
    }
}
