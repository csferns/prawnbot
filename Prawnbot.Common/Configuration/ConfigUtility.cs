using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Prawnbot.Common.Configuration
{
    public class ConfigUtility : IConfigUtility
    {
        private readonly IConfigurationRoot Config;

        public ConfigUtility()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("Configuration\\appsettings.json", optional: false);

            Config = builder.Build();
        }

        public string GetConfig(string parent, [CallerMemberName]string memberName = "")
        {
            string value = Config.GetSection(parent).GetSection(memberName).Value;
            return string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value) ? null : value;
        }

        public static string GetStaticConfig(string parent, [CallerMemberName]string memberName = "")
        {
            IConfigUtility configUtility = new ConfigUtility();
            return configUtility.GetConfig(parent, memberName);
        }

        public string TextFileDirectory => GetConfig("Generic") ?? Environment.CurrentDirectory + "\\Text Files";

        public string CommandDelimiter => GetConfig("Generic") ?? "p!";

        public bool AllowEventListeners => bool.Parse(GetConfig("EventListeners"));

        public bool ProfanityFilter => bool.Parse(GetConfig("EventListeners"));

        public bool DadMode => bool.Parse(GetConfig("EventListeners"));

        public bool YottaMode => bool.Parse(GetConfig("EventListeners"));

        public bool EmojiRepeat => bool.Parse(GetConfig("EventListeners"));

        public bool UserJoined => bool.Parse(GetConfig("EventListeners"));

        public bool UserBanned => bool.Parse(GetConfig("EventListeners"));

        public bool UserUnbanned => bool.Parse(GetConfig("EventListeners"));

        public bool MessageDeleted => bool.Parse(GetConfig("EventListeners"));

        public bool JoinedGuild => bool.Parse(GetConfig("EventListeners"));

        public string MicrosoftSpeechServicesEndpoint => GetConfig("Endpoints");

        public string MicrosoftSpeechServicesRegion => GetConfig("Keys");

        public string GiphyEndpoint => GetConfig("Endpoints");

        public string MicrosoftTranslateEndpoint => GetConfig("Endpoints");

        public string R34Endpoint => GetConfig("Endpoints");

        /// <summary>
        /// Endpoint to be used when calling off to the Overwatch stats API
        /// </summary>
        public string OverwatchStatsEndpoint => GetConfig("Endpoints");

        public string ProfanityFilterEndpoint => GetConfig("Endpoints");

        /// <summary>
        /// If the database connection is intitialised
        /// </summary>
        public static bool UseDatabaseConnection => bool.Parse(GetStaticConfig("Generic"));

        /// <summary>
        /// Connection string for local database storage
        /// If UseDatabaseConnection is true, this is required.
        /// </summary>
        public static string DatabaseConnectionString => GetStaticConfig("ConnectionStrings");

        /// <summary>
        /// The token to be used when connecting the bot to the Discord API
        /// </summary>
        public string BotToken => GetConfig("Keys");

        public string BlobStoreConnectionString => GetConfig("ConnectionStrings");

        public string GiphyAPIKey => GetConfig("Keys");

        public string TranslateAPIKey => GetConfig("Keys");

        public string MicrosoftSpeechServicesKey => GetConfig("Keys");

        public string GoogleAPIKey => GetConfig("Keys");

        public string GoogleApplicationName => GetConfig("Keys");

        public string GoogleClientSecret => GetConfig("Keys");

        /// <summary>
        /// Gets the text colour of the Console
        /// </summary>
        public ConsoleColor ConsoleForeground => Enum.Parse<ConsoleColor>(GetConfig("Generic"));

        /// <summary>
        /// Gets the background colour of the Console
        /// </summary>
        public ConsoleColor ConsoleBackground => Enum.Parse<ConsoleColor>(GetConfig("Generic"));

        /// <summary>
        /// Gets the Title of the Console
        /// </summary>
        public string ConsoleTitle => GetConfig("Generic");

        /// <summary>
        /// Gets the encoding to be used when displaying text in the Console
        /// </summary>
        public Encoding ConsoleEncoding => Encoding.GetEncoding(GetConfig("Generic"));
    }
}
