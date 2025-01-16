using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("Topics")]
    public class TopicEntity : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        [MaxLength(1000)]
        public string Term { get; set; } = "";
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public ICollection<SubTopicEntity>? SubTopics { get; set; }
    }
}
