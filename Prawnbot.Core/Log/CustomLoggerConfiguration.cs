using Microsoft.Extensions.Logging;
using System;

namespace Prawnbot.Core.Log
{
    public class CustomLoggerConfiguration
    {
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public ConsoleColor ConsoleColour { get; set; }
        public string FileName { get; set; }
    }
}
