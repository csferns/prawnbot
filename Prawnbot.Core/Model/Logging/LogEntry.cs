using Discord;
using System;
using System.Diagnostics;
using System.Globalization;

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

            bool parseSuccess = Enum.TryParse<TraceEventType>(logMessage.Severity.ToString(), ignoreCase: true, out TraceEventType severity);

            LogSeverity = parseSuccess ? severity : TraceEventType.Information;

            Area = "Discord";
        }
        
        public string Message { get; set; }
        public string Area { get; set; }
        public TraceEventType LogSeverity { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture), LogSeverity.ToString(), Area, Message);
        }
    }
}
