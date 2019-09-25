using Discord;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Prawnbot.Core.Log
{
    public interface ILogging
    {
        Task Client_Log(LogMessage message);
        Task PopulateEventLogAsync(LogMessage message);
        Task PopulateMessageLogAsync(LogMessage message);
        Task LogCommandUseAsync(string username, string guild, string messageContent);
        Task PopulateTranslationLogAsync(Bunch<TranslateData> translation);
    }

    public class Logging : BaseBL, ILogging
    {
        private readonly IFileService fileService;
        public Logging(IFileService fileService)
        {
            this.fileService = fileService;
        }

        private string GetLogMessageString(LogMessage message)
        {
            return $"{DateTime.Now.ToString("dd/MM/yyy hh:mm:ss tt", CultureInfo.InvariantCulture)} {message.ToString(prependTimestamp: false, padSource: 20)}";
        }

        public async Task Client_Log(LogMessage message)
        {
            await PopulateEventLogAsync(message);
        }

        public async Task PopulateEventLogAsync(LogMessage message)
        {
            await fileService.WriteToFileAsync(GetLogMessageString(message), "EventLogs.txt");
            await Console.Out.WriteLineAsync(GetLogMessageString(message));
        }

        public async Task PopulateMessageLogAsync(LogMessage message)
        {
            await fileService.WriteToFileAsync(GetLogMessageString(message), "MessageLogs.txt");
            await Console.Out.WriteLineAsync(GetLogMessageString(message));
        }

        public async Task LogCommandUseAsync(string username, string guild, string messageContent)
        {
            await PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "Command_Usage", $"Message recieved from {username}: ({guild}) \"{messageContent}\""));
        }

        public async Task PopulateTranslationLogAsync(Bunch<TranslateData> translation)
        {
            foreach (TranslateData item in translation)
            {
                Translation firstTranslation = item.translations.FirstOrDefault();
                await fileService.WriteToFileAsync($"{firstTranslation.to} : {firstTranslation.text}", "TranslationLog.txt");
            }

            await PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "Translation_Log", "Translations logged"));
        }
    }
}
