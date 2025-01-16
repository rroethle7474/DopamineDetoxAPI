using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using System.Threading.Tasks;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface ISubTopicService
    {
        Task<SubTopic> CreateSubTopicAsync(SubTopic topic, CancellationToken cancellationToken = default);
        Task<SubTopic> GetSubTopicAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SubTopic>> GetSubTopicsAsync(GetSubTopicsRequest request, CancellationToken cancellationToken = default);
        Task<SubTopic> UpdateSubTopicAsync(int id, SubTopic topic, CancellationToken cancellationToken = default);
        Task DeleteSubTopicAsync(int id, CancellationToken cancellationToken = default);
        Task<SubTopic> GetSubTopicByUserIdAndTermAsync(string userId, string term, CancellationToken cancellationToken = default);
        bool ValidateCreateSubTopicDto(SubTopicDto subTopicDto);
        Task DeleteSubTopicsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
