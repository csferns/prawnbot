using Discord;
using Prawnbot.Common.DTOs.API.Translation;
using Prawnbot.Core.Custom.Collections;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Prawnbot.Logging 
{
    /// <summary>
    /// Methods to allow the logging of exceptions and general information to a specified source
    /// </summary>
    public interface ILogging
    {
        Task Client_Log(LogMessage message);
        void Log_Warning(string message, bool updateConsole = true, [CallerMemberName]string codeArea = "");
        void Log_Info(string message, bool updateConsole = true, [CallerMemberName]string codeArea = "");
        void Log_Exception(Exception e, bool updateConsole = true, string optionalMessage = null, [CallerMemberName]string codeArea = "");
        void Log_Debug(string message, [CallerMemberName]string codeArea = "");
        Task PopulateTranslationLogAsync(Bunch<TranslateData> translation);
    }
}
