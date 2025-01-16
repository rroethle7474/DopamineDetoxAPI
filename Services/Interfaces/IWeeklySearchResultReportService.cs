using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IWeeklySearchResultReportService
    {
        Task<WeeklySearchResultReport> CreateWeeklySearchResultReportAsync(WeeklySearchResultReport weeklySearchResultReport, CancellationToken cancellationToken = default);
        Task<WeeklySearchResultReport> GetWeeklySearchResultReportAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<WeeklySearchResultReport>> GetWeeklySearchResultReportsAsync(GetWeeklySearchResultReportsRequest request, CancellationToken cancellationToken = default);
        Task<WeeklySearchResultReport> UpdateWeeklySearchResultReportAsync(int id, WeeklySearchResultReport weeklySearchResultReport, CancellationToken cancellationToken = default);
        Task DeleteWeeklySearchResultReportAsync(int id, CancellationToken cancellationToken = default);
    }
}
