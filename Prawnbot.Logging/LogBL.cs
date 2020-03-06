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
        private readonly IFileService fileService;
        public LogBL(IFileService fileService)
        {
            this.fileService = fileService;
        }

        public async Task Client_Log(LogMessage message)
        {
            LogEntry logEntry = new LogEntry(message);
            Log(logEntry);
        }

        public void Log_Warning(string message, bool updateConsole = true, [CallerMemberName]string codeArea = "")
        {
            LogEntry logEntry = new LogEntry()
            {
                Area = codeArea,
                Message = message,
                LogSeverity = TraceEventType.Warning
            };

            Log(logEntry, updateConsole);
        }

        public void Log_Info(string message, bool updateConsole = true, [CallerMemberName]string codeArea = "")
        {
            LogEntry logEntry = new LogEntry()
            {
                Area = codeArea,
                Message = message,
                LogSeverity = TraceEventType.Information
            };

            Log(logEntry, updateConsole);
        }

        public void Log_Exception(Exception e, bool updateConsole = true, string optionalMessage = null, [CallerMemberName]string codeArea = "")
        {
            LogEntry logEntry = new LogEntry()
            {
                Area = codeArea,
                Message = optionalMessage == null
                    ? string.Format("{0} {1}", e.Message, e.StackTrace)
                    : string.Format("{2}: {0} {1}", e.Message, e.StackTrace, optionalMessage),
                LogSeverity = TraceEventType.Error
            };

            Log(logEntry, updateConsole);
        }

        public void Log_Debug(string message, [CallerMemberName]string codeArea = "")
        {
            LogEntry logEntry = new LogEntry()
            {
                Area = codeArea,
                Message = message,
                LogSeverity = TraceEventType.Information
            };

            Log(logEntry, false);
        }

        private void Log(LogEntry message, bool updateConsole = true)
        {
            try
            {
                Task.Run(async () =>
                {
                    await fileService.WriteToLogFileAsync(message.ToString());

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

        public async Task PopulateTranslationLogAsync(IBunch<TranslateData> translation)
        {
            foreach (TranslateData item in translation)
            {
                Translation firstTranslation = item.translations.FirstOrDefault();
                await fileService.WriteToFileAsync($"{firstTranslation.to} : {firstTranslation.text}", "TranslationLog.txt");
            }

            Log_Info("Translations logged");
        }
    }
}
