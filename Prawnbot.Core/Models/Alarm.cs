using System;

namespace Prawnbot.Core.Models
{
    public class Alarm
    {
        public DateTime AlarmTime { get; set; }
        public string User { get; set; }
        public string AlarmName { get; set; }
    }
}
