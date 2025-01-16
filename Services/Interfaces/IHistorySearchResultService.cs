using DopamineDetox.Domain.Models;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface IHistorySearchResultService
    {
        Task<HistorySearchResult> CreateHistorySearchResultAsync(HistorySearchResult historySearchResult, CancellationToken cancellationToken = default);
        Task<HistorySearchResult> GetHistorySearchResultAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<HistorySearchResult>> GetHistorySearchResultsAsync(CancellationToken cancellationToken = default);
        Task<HistorySearchResult> UpdateHistorySearchResultAsync(int id, HistorySearchResult historySearchResult, CancellationToken cancellationToken = default);
        Task DeleteHistorySearchResultAsync(int id, CancellationToken cancellationToken = default);
    }
}
