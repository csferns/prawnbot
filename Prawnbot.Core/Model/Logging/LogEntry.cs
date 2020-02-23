using Discord;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Prawnbot.Core.Model.Logging
{
    public class LogEntry
    {
        public LogEntry()
        {

        }

        public LogEntry(LogMessage logMessage)
        {
            Message = logMessage.Message;

            Enum.TryParse(typeof(TraceEventType), logMessage.Severity.ToString(), out object severity);
            LogSeverity = (TraceEventType)severity;
        }

        public LogEntry(Exception exception, [CallerMemberName]string codeArea = "", string optionalMessage = null)
        {
            Message = optionalMessage == null 
                ? string.Format("{0} {1}", exception.Message, exception.StackTrace) 
                : string.Format("{2}: {0} {1}", exception.Message, exception.StackTrace, optionalMessage);

            Area = codeArea;
            LogSeverity = TraceEventType.Error;
        }
        
        public string Message { get; set; }
        public string Area { get; set; }
        public TraceEventType LogSeverity { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture), Area ?? "Discord", Message);
        }
    }
}
