﻿using Discord;
using Prawnbot.Core.BusinessLayer;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Interfaces;
using Prawnbot.Core.Model.API.Translation;
using Prawnbot.Core.Model.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Prawnbot.Core.Log
{
    public class Logging : BaseBL, ILogging
    {
        /// <summary>
        /// Constant string used for the file name to target for the event logs file
        /// </summary>
        private const string LogFileName = "EventLogs.txt";

        private readonly IFileService fileService;
        public Logging(IFileService fileService)
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
            catch (NullReferenceException)
            {
                await FailoverLog(message);
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

        public static async Task FailoverLog(LogEntry message)
        {
            await FileBL.FailoverWriteToFileAsync(message.ToString(), LogFileName);
        }
    }
}
