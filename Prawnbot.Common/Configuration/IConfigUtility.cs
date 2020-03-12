using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Prawnbot.Common.Configuration
{
    public interface IConfigUtility
    {
        string GetConfig(string parent, [CallerMemberName]string memberName = "");
        string TextFileDirectory { get; }
        string CommandDelimiter { get; }
        bool AllowEventListeners { get; }
        bool ProfanityFilter { get; }
        bool DadMode { get; }
        bool YottaMode { get; }
        bool EmojiRepeat { get; }
        bool UserJoined { get; }
        bool UserBanned { get; }
        bool UserUnbanned { get; }
        bool MessageDeleted { get; }
        bool JoinedGuild { get; }
        string MicrosoftSpeechServicesEndpoint { get; }
        string MicrosoftSpeechServicesRegion { get; }
        string GiphyEndpoint { get; }
        string MicrosoftTranslateEndpoint { get; }
        string R34Endpoint { get; }
        string OverwatchStatsEndpoint { get; }
        string ProfanityFilterEndpoint { get; }
        static bool UseDatabaseConnection { get; }
        static string DatabaseConnectionString { get; }
        string BotToken { get; }
        string BlobStoreConnectionString { get; }
        string GiphyAPIKey { get; }
        string TranslateAPIKey { get; }
        string MicrosoftSpeechServicesKey { get; }
        string GoogleAPIKey { get; }
        string GoogleApplicationName { get; }
        string GoogleClientSecret { get; }
        ConsoleColor ConsoleForeground { get; }
        ConsoleColor ConsoleBackground { get; }
        string ConsoleTitle { get; }
        Encoding ConsoleEncoding { get; }
    }
}
