using DopamineDetox.Domain.Models;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IDefaultTopicService
    {
        Task<DefaultTopic> CreateDefaultTopicAsync(DefaultTopic topic, CancellationToken cancellationToken = default);
        Task<DefaultTopic> GetDefaultTopicAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<DefaultTopic>> GetDefaultTopicsAsync(CancellationToken cancellationToken = default);
        Task<DefaultTopic> UpdateDefaultTopicAsync(int id, DefaultTopic topic, CancellationToken cancellationToken = default);
        Task DeleteDefaultTopicAsync(int id, CancellationToken cancellationToken = default);
    }
}
