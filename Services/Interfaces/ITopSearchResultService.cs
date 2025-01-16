using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface ITopSearchResultService
    {
        Task<TopSearchResult> CreateTopSearchResultAsync(TopSearchResult topSearchResult, CancellationToken cancellationToken = default);
        Task<TopSearchResult> GetTopSearchResultAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TopSearchResult>> GetTopSearchResultsAsync(GetTopSearchResultsRequest request, CancellationToken cancellationToken = default);
        Task<TopSearchResult> UpdateTopSearchResultAsync(int id, TopSearchResult topSearchResult, CancellationToken cancellationToken = default);
        Task DeleteTopSearchResultAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteTopSearchResultsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetTopSearchResultsUserList(CancellationToken cancellationToken = default);
        Task<int> DeleteTopSearchResultsFromDateAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<int> DeleteTopSearchResultsBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
