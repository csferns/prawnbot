using Prawnbot.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Prawnbot.Data.Entities
{
    public class Yotta
    {
        [Key]
        public int PrependId { get; set; }
        public PrependEnum PrependValue { get; set; }
        public int Count { get; set; }
    }
}
