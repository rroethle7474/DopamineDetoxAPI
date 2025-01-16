using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("Channels")]
    public class ChannelEntity : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        [StringLength(100)]
        public string ChannelName { get; set; }
        [Required]
        [StringLength(500)]
        public string? Identifier { get; set; }
        public string? Description { get; set; }
        [Required]
        public int ContentTypeId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [ForeignKey("ContentTypeId")]
        public ContentTypeEntity ContentType { get; set; }
        [Required]
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
