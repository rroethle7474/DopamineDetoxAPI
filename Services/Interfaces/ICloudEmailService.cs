using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface ICloudEmailService
    {
        Task<bool> SendMVPWeeklyEmail(User user, List<SearchResult> results, CancellationToken cancellationToken = default);
        Task<bool> SendPasswordResetEmail(ApplicationUser user, string encodedToken, string baseUrl, CancellationToken cancellationToken = default);
    }

}