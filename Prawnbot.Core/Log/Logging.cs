using Discord;
using Prawnbot.Core.Base;
using Prawnbot.Core.LocalFileAccess;
using Prawnbot.Data.Models.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Prawnbot.Core.Log
{    
    public interface ILogging
    {
        Task PopulateEventLog(LogMessage message);
        Task PopulateMessageLog(LogMessage message);
        Task PopulateTranslationLog(List<TranslateData> translation);
    }

    public class Logging : BaseBl, ILogging
    {
        public Logging()
        {

        }

        public async Task PopulateEventLog(LogMessage message)
        {
            await _fileService.WriteToFile(message.ToString(timestampKind: DateTimeKind.Local), "EventLogs.txt");
            Console.WriteLine(message.ToString(timestampKind: DateTimeKind.Local));
        }

        public async Task PopulateMessageLog(LogMessage message)
        {
            await _fileService.WriteToFile(message.ToString(timestampKind: DateTimeKind.Local), "MessageLogs.txt");
            Console.WriteLine(message.ToString(timestampKind: DateTimeKind.Local));
        }

        public async Task PopulateTranslationLog(List<TranslateData> translation)
        {
            foreach (var item in translation)
            {
                foreach (var innerTranslation in item.translations)
                {
                    await _fileService.WriteToFile($"{innerTranslation.to} : {innerTranslation.text}", "TranslationLog.txt");
                }
                
            }
        }
    }
}
