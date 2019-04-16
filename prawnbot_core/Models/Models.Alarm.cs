using System;

namespace prawnbot_core.Models
{
    public class Alarm
    {
        public DateTime AlarmTime { get; set; }
        public string User { get; set; }
        public string AlarmName { get; set; }
    }
}
