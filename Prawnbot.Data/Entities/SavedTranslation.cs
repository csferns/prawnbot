using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prawnbot.Data.Entities
{
    [Table("SavedTranslation", Schema = "core")]
    public class SavedTranslation
    {
        [Key]
        public int TranslationId { get; set; }
        public string Translation { get; set; }
        public string ToLanguage { get; set; }
    }
}
