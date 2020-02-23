using Discord;
using Prawnbot.Core.Collections;
using Prawnbot.Core.Model.API.Translation;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Prawnbot.Core.Interfaces
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
