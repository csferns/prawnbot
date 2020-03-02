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
        Task Log_Warning(string message, bool updateConsole = true, [CallerMemberName]string codeArea = ""); 
        Task Log_Info(string message, bool updateConsole = true, [CallerMemberName]string codeArea = "");
        Task Log_Exception(Exception e, bool updateConsole = true, string optionalMessage = null, [CallerMemberName]string codeArea = "");
        Task Log_Debug(string message, [CallerMemberName]string codeArea = "");
        Task PopulateTranslationLogAsync(Bunch<TranslateData> translation);
    }
}
