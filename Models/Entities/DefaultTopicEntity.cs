using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("DefaultTopics")]
    public class DefaultTopicEntity : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Term { get; set; } = "";

        public bool ExcludeFromTwitter { get; set; } = false;
    }
}
