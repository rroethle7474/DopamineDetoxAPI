using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Models.Entities;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
        Task SendEmailsAsync(IEnumerable<string> to, string subject, string htmlContent);
        Task<bool> SendMVPWeeklyEmail(User user, List<SearchResult> results, CancellationToken cancellationToken = default);
        Task<bool> SendPasswordResetEmail(ApplicationUser user, string encodedToken, string baseUrl, CancellationToken cancellationToken = default);
    }
}
