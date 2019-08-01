using Discord;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Prawnbot.Core.Log
{
    public interface ILogging
    {
        Task PopulateEventLogAsync(LogMessage message);
        Task PopulateMessageLogAsync(LogMessage message);
        Task LogCommandUseAsync(string username, string guild, string messageContent);
        Task PopulateTranslationLogAsync(List<TranslateData> translation);
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
            return $"{DateTime.Now.ToString("dd/MM/yyy hh:mm:ss tt", CultureInfo.InvariantCulture)} {message.ToString(prependTimestamp: false)}";
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
            await PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "LogCommandUse", $"Message recieved from {username}: ({guild}) \"{messageContent}\""));
        }

        public async Task PopulateTranslationLogAsync(List<TranslateData> translation)
        {
            foreach (TranslateData item in translation)
            {
                foreach (Translation innerTranslation in item.translations)
                {
                    await fileService.WriteToFileAsync($"{innerTranslation.to} : {innerTranslation.text}", "TranslationLog.txt");
                }
            }

            await PopulateEventLogAsync(new LogMessage(LogSeverity.Info, "Logging", "Translations logged"));
        }
    }
}
