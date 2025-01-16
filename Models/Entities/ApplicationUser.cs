using Microsoft.AspNetCore.Identity;

namespace DopamineDetoxAPI.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? GoogleEmail { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ICollection<TopicEntity>? Topics { get; set; }
        public ICollection<SubTopicEntity>? SubTopics { get; set; }
        public ICollection<ChannelEntity>? Channels { get; set; }
        public ICollection<NoteEntity>? Notes { get; set; }
        public ICollection<TopSearchResultEntity>? TopSearchResults { get; set; }
        
    }
}
