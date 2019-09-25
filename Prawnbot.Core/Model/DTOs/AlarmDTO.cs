using System;

namespace Prawnbot.Core.Model.DTOs
{
    public class AlarmDTO
    {
        public DateTime AlarmTime { get; set; }
        public string User { get; set; }
        public string AlarmName { get; set; }
    }
}
