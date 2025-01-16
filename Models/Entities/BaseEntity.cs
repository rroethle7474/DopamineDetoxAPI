namespace DopamineDetoxAPI.Models.Entities
{
    public class BaseEntity
    {
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; } = DateTime.Now;
    }
}
