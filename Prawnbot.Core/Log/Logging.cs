using Discord;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prawnbot.Core.Log
{
    public interface ILogging
    {
        Task PopulateEventLog(LogMessage message);
        Task PopulateMessageLog(LogMessage message);
        Task PopulateTranslationLog(List<TranslateData> translation);
    }

    public class Logging : BaseBL, ILogging 
    {
        private readonly IFileService fileService;
        public Logging(IFileService fileService) 
        {
            this.fileService = fileService;
        }

        public async Task PopulateEventLog(LogMessage message)
        {
            await fileService.WriteToFile(message.ToString(timestampKind: DateTimeKind.Local), "EventLogs.txt");
            await Console.Out.WriteLineAsync(message.ToString(timestampKind: DateTimeKind.Local));
        }

        public async Task PopulateMessageLog(LogMessage message)
        {
            await fileService.WriteToFile(message.ToString(timestampKind: DateTimeKind.Local), "MessageLogs.txt");
            await Console.Out.WriteLineAsync(message.ToString(timestampKind: DateTimeKind.Local));
        }

        public async Task PopulateTranslationLog(List<TranslateData> translation)
        {
            foreach (TranslateData item in translation)
            {
                foreach (Translation innerTranslation in item.translations)
                {
                    await fileService.WriteToFile($"{innerTranslation.to} : {innerTranslation.text}", "TranslationLog.txt");
                }
            }

            await PopulateEventLog(new LogMessage(LogSeverity.Info, "Logging", "Translations logged"));
        }
    }
}
