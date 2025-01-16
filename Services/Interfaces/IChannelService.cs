using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IChannelService
    {
        Task<Channel> CreateChannelAsync(Channel channel, CancellationToken cancellationToken = default);
        Task<Channel> GetChannelAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Channel>> GetChannelsAsync(GetChannelsRequest request, CancellationToken cancellationToken = default);
        Task<Channel> UpdateChannelAsync(int id, Channel channel, CancellationToken cancellationToken = default);
        Task DeleteChannelAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteChannelsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
