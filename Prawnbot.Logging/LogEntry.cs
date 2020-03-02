using Discord;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Prawnbot.Logging
{
    public class LogEntry
    {
        public LogEntry()
        {

        }

        public LogEntry(LogMessage logMessage)
        {
            Message = logMessage.Message;
            DiscordLogSeverity = logMessage.Severity;

            Area = "Discord";
            FromDiscord = true;
        }

        public bool FromDiscord { get; set; }
        
        public string Message { get; set; }
        public string Area { get; set; }
        public TraceEventType LogSeverity { get; set; }
        public LogSeverity DiscordLogSeverity { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture), FromDiscord ? DiscordLogSeverity.ToString() : LogSeverity.ToString(), Area, Message);
        }
    }
}
