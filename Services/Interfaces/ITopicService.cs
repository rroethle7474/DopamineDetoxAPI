using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface ITopicService
    {
        Task<Topic> CreateTopicAsync(Topic topic, CancellationToken cancellationToken = default);
        Task<Topic> GetTopicAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Topic>> GetTopicsAsync(GetTopicsRequest request, CancellationToken cancellationToken = default);
        Task<Topic> UpdateTopicAsync(int id, Topic topic, CancellationToken cancellationToken = default);
        Task DeleteTopicAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteTopicsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
