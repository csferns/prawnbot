using Prawnbot.Common.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prawnbot.Data.Entities
{
    [Table("Yotta", Schema = "core")]
    public class Yotta
    {
        [Key]
        public int PrependId { get; set; }
        public PrependEnum PrependValue { get; set; }
        public int Count { get; set; }
    }
}
