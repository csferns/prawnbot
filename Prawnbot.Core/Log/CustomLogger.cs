using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;

namespace Prawnbot.Core.Log
{
    public class CustomLogger : ILogger
    {
        private readonly string _name;
        private readonly CustomLoggerConfiguration configuration;

        public CustomLogger(string name, CustomLoggerConfiguration configuration)
        {
            this._name = name;
            this.configuration = configuration;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == configuration.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message = string.Format(CultureInfo.CurrentCulture, "{0:dd/MM/yyyy hh:mm:ss tt}: {1} {2} {3} {4}", DateTime.Now, logLevel, _name, formatter(state, exception), exception?.StackTrace);

            Console.ForegroundColor = configuration.ConsoleColour;
            Console.Out.WriteLine(message);
            Console.ResetColor();

            if (!string.IsNullOrEmpty(configuration.FileName))
            {
                using (FileStream fs = new FileStream(configuration.FileName, FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}
