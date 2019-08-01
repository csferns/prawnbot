using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prawnbot.Data.Entities
{
    [Table("BotResponse", Schema = "core")]
    public class Alarm
    {
        public DateTime AlarmTime { get; set; }
        public string User { get; set; }
        public string AlarmName { get; set; }
        public bool Repeat { get; set; }
    }
}
