using System;

namespace Prawnbot.Common.DTOs
{
    public class AlarmDTO
    {
        public DateTime AlarmTime { get; set; }
        public string User { get; set; }
        public string AlarmName { get; set; }
    }
}
