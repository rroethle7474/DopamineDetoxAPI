using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<(string htmlBodyContent, string subject)> BuildUserMVPWeeklyEmail(User user, List<SearchResult> articles, CancellationToken cancellationToken = default);

        Task<(string htmlBodyContent, string subject)> BuildResetPasswordEmail(ApplicationUser user, string encodedToken, string baseUrl, CancellationToken cancellationToken = default);
    }
}