using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DopamineDetoxAPI.Models.Entities
{
    [Table("WeeklySearchResultReports")]
    public class WeeklySearchResultReportEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public bool IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}

