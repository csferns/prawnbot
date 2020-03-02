using Discord;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Core.Custom.Collections;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using Prawnbot.FileHandling.Interfaces;

namespace Prawnbot.Logging
{
    public class LogBL : ILogging
    {
        /// <summary>
        /// Constant string used for the file name to target for the event logs file
        /// </summary>
        private const string LogFileName = "EventLogs.txt";

        private readonly IFileService fileService;
        public LogBL(IFileService fileService)
        {
            this.fileService = fileService;
        }

        public async Task Client_Log(LogMessage message)
        {
            LogEntry logEntry = new LogEntry(message);
            await Log(logEntry);
        }

        public async Task Log_Warning(string message, bool updateConsole = true, [CallerMemberName]string codeArea = "")
        {
            LogEntry logEntry = new LogEntry()
            {
                Area = codeArea,
                Message = message,
                LogSeverity = TraceEventType.Warning
            };

            await Log(logEntry, updateConsole);
        }

        public async Task Log_Info(string message, bool updateConsole = true, [CallerMemberName]string codeArea = "")
        {
            LogEntry logEntry = new LogEntry()
            {
                Area = codeArea,
                Message = message,
                LogSeverity = TraceEventType.Information
            };

            await Log(logEntry, updateConsole);
        }

        public async Task Log_Exception(Exception e, bool updateConsole = true, string optionalMessage = null, [CallerMemberName]string codeArea = "")
        {
            LogEntry logEntry = new LogEntry()
            {
                Area = codeArea,
                Message = optionalMessage == null
                    ? string.Format("{0} {1}", e.Message, e.StackTrace)
                    : string.Format("{2}: {0} {1}", e.Message, e.StackTrace, optionalMessage),
                LogSeverity = TraceEventType.Error
            };

            await Log(logEntry, updateConsole);
        }

        public async Task Log_Debug(string message, [CallerMemberName]string codeArea = "")
        {
            LogEntry logEntry = new LogEntry()
            {
                Area = codeArea,
                Message = message,
                LogSeverity = TraceEventType.Information
            };

            await Log(logEntry, false);
        }

        private async Task Log(LogEntry message, bool updateConsole = true)
        {
            try
            {
                await Task.Run(async () =>
                {
                    await fileService.WriteToFileAsync(message.ToString(), LogFileName);

                    if (updateConsole)
                    {
                        await Console.Out.WriteLineAsync(message.ToString());
                    }
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task PopulateTranslationLogAsync(Bunch<TranslateData> translation)
        {
            foreach (TranslateData item in translation)
            {
                Translation firstTranslation = item.translations.FirstOrDefault();
                await fileService.WriteToFileAsync($"{firstTranslation.to} : {firstTranslation.text}", "TranslationLog.txt");
            }

            await Log_Info("Translations logged");
        }
    }
}
