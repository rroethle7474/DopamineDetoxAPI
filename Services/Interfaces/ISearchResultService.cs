using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;

namespace DopamineDetoxAPI.Services.Interfaces
{
    public interface ISearchResultService
    {
        Task<SearchResult> CreateSearchResultAsync(SearchResult searchResult, CancellationToken cancellationToken = default);
        Task<SearchResultsResponseDto> AddMultipleSearchResultsAsync(IEnumerable<SearchResult> searchResults, CancellationToken cancellationToken = default);
        Task<SearchResult> GetSearchResultAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SearchResult>> GetSearchResultsAsync(GetSearchResultsRequest request, CancellationToken cancellationToken = default);
        Task<SearchResult> UpdateSearchResultAsync(int id, SearchResult searchResult, CancellationToken cancellationToken = default);
        Task DeleteSearchResultAsync(int id, CancellationToken cancellationToken = default);
        Task<int> DeleteSearchResultsFromDateAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<int> DeleteSearchResultsBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default);
    }
}
