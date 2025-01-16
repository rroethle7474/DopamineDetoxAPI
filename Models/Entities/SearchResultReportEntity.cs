using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("SearchResultReports")]
    public class SearchResultReportEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ContentTypeId { get; set; }

        [Required]
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }

        public bool IsDefaultReport { get; set; } = false;
        public bool IsChannelReport { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("ContentTypeId")]
        public virtual ContentTypeEntity ContentType { get; set; }
    }
}

