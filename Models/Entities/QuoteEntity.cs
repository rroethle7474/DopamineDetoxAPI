using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("Quotes")]
    public class QuoteEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string QuoteText { get; set; }

        [Required]
        public byte[] QuoteImage { get; set; }

        [Required]
        public DateTime QuoteDateFetched { get; set; }
    }
}
