using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    public class TopSearchResultEntity : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int SearchResultId { get; set; }

        //Foreign Key Constrains
        [Required]
        [ForeignKey("SearchResultId")]
        public SearchResultEntity SearchResult { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
