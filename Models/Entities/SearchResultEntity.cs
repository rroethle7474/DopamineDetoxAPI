using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("SearchResults")]
    public class SearchResultEntity
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
        public string? EmbeddedHtml { get; set; }
        public bool IsHomePage { get; set; } = false;
        public bool? TopSearchResult { get; set; } = false;

        [Required]
        public DateTime PublishedAt { get; set; } = DateTime.Now;
        [Required]
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool? IsChannel { get; set; } = false;


        // Xreferences
        [StringLength(1000)]
        public string? Term { get; set; }
        [StringLength(100)]
        public string? ChannelName { get; set; }
        [Required]
        public int ContentTypeId { get; set; }


        [ForeignKey("ContentTypeId")]
        public ContentTypeEntity? ContentType { get; set; }


        public ICollection<NoteEntity>? Notes { get; set; }
        public ICollection<TopSearchResultEntity>? TopSearchResults { get; set; }
    }
}
