using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("SubTopics")]
    public class SubTopicEntity : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        [MaxLength(1000)]
        public string Term { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int TopicId { get; set; }

        public bool ExcludeFromTwitter { get; set; } = false;

        [ForeignKey("TopicId")]
        public TopicEntity Topic { get; set; }  // Navigation property

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
