using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("ContentTypes")]
    public class ContentTypeEntity : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(1000)]
        public string? Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string? Description { get; set; }
        public ICollection<ChannelEntity>? Channels { get; set; }
    }
}
