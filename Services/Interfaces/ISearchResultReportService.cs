using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface ISearchResultReportService
    {
        Task<SearchResultReport> CreateSearchResultReportAsync(SearchResultReport searchResultReport, CancellationToken cancellationToken = default);
        Task<SearchResultReport> GetSearchResultReportAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SearchResultReport>> GetSearchResultReportsAsync(GetSearchResultReportRequest request, CancellationToken cancellationToken = default);
        Task<SearchResultReport> UpdateSearchResultReportAsync(int id, SearchResultReport searchResultReport, CancellationToken cancellationToken = default);
        Task DeleteSearchResultReportAsync(int id, CancellationToken cancellationToken = default);
        Task<int> DeleteSearchResultReportsFromDateAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<int> DeleteSearchResultReportsBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
