using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    // inheriting from base entity since I want to know when this was moved to History
    [Table("HistorySearchResults")]
    public class HistorySearchResultEntity : BaseEntity
    {
        // non-nullable fields
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        // YouTube Channel Name or Twitter UserName
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(500)]
        public string? Url { get; set; }
        [StringLength(500)]
        public string? EmbedUrl { get; set; }
        [StringLength(100)]
        public string? VideoId { get; set; }
        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        public DateTime? PublishedAt { get; set; }


        // Xreferences
        public string? SubTopicTerm { get; set; }
        public string? ChannelName { get; set; }
        public string? ContentTypeName { get; set; }
    }
}