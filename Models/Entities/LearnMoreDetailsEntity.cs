using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("LearnMoreDetails")]
    public class LearnMoreDetailsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Introduction { get; set; }

        [Required]
        public string Items { get; set; }

        [Required]
        public DateTime QuoteDateFetched { get; set; }
    }
}
