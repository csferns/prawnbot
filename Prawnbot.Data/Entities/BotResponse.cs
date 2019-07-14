using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prawnbot.Data.Entities
{
    [Table("BotResponse", Schema = "core")]
    public class BotResponse
    {
        [Key]
        public int ResponseId { get; set; }
        [StringLength(2000)]
        public string LookupValue { get; set; }
        [StringLength(2000)]
        public string ReplyValue { get; set; }
    }
}
