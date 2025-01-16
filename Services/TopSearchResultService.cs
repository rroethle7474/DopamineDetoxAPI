using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class TopSearchResultService : ITopSearchResultService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TopSearchResultService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TopSearchResult> CreateTopSearchResultAsync(TopSearchResult topSearchResult, CancellationToken cancellationToken = default)
        {
            var topSearchResultEntity = _mapper.Map<TopSearchResultEntity>(topSearchResult);

            _context.TopSearchResults.Add(topSearchResultEntity);
            await _context.SaveChangesAsync(cancellationToken);

            topSearchResult.Id = topSearchResultEntity.Id;
            topSearchResult.SearchResult = _mapper.Map<SearchResult>(topSearchResultEntity.SearchResult);

            return topSearchResult;
        }

        public async Task<TopSearchResult> GetTopSearchResultAsync(int id, CancellationToken cancellationToken = default)
        {
            var topSearchResultEntity = await _context.TopSearchResults
                                        .Include(tsr => tsr.SearchResult) // Include the related SearchResult entity
                                        .FirstOrDefaultAsync(tsr => tsr.Id == id, cancellationToken);

            if (topSearchResultEntity == null)
            {
                throw new Exception($"TopSearchResult with id {id} not found");
            }

            return _mapper.Map<TopSearchResult>(topSearchResultEntity);
        }

        public async Task<IEnumerable<TopSearchResult>> GetTopSearchResultsAsync(GetTopSearchResultsRequest request, CancellationToken cancellationToken = default)
        {
            var query = _context.TopSearchResults
                        .Include(tsr => tsr.SearchResult) // Include the related SearchResult entity
                        .AsQueryable();

            if (request.Id.HasValue)
            {
                query = query.Where(tsr => tsr.Id == request.Id.Value);
            }

            if (!string.IsNullOrEmpty(request.UserId))
            {
                query = query.Where(tsr => tsr.UserId == request.UserId);
            }

            if (request.SearchResultId.HasValue)
            {
                query = query.Where(tsr => tsr.SearchResultId == request.SearchResultId.Value);
            }

            var topSearchResultEntities = await query.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TopSearchResult>>(topSearchResultEntities);
        }

        public async Task<TopSearchResult> UpdateTopSearchResultAsync(int id, TopSearchResult topSearchResult, CancellationToken cancellationToken = default)
        {
            var topSearchResultEntity = await _context.TopSearchResults
                .Include(tsr => tsr.SearchResult)
                .FirstOrDefaultAsync(tsr => tsr.Id == id, cancellationToken);

            if (topSearchResultEntity == null)
            {
                throw new Exception($"TopSearchResult with id {id} not found");
            }

            _mapper.Map(topSearchResult, topSearchResultEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return topSearchResult;
        }

        public async Task DeleteTopSearchResultAsync(int id, CancellationToken cancellationToken = default)
        {
            var topSearchResultEntity = await _context.TopSearchResults.FindAsync(new object[] { id }, cancellationToken);

            if (topSearchResultEntity == null)
            {
                throw new Exception($"TopSearchResult with id {id} not found");
            }

            _context.TopSearchResults.Remove(topSearchResultEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteTopSearchResultsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var topSearchResultEntities = await _context.TopSearchResults.Where(tsr => tsr.UserId == userId).ToListAsync(cancellationToken);

            if (topSearchResultEntities == null || !topSearchResultEntities.Any())
            {
                return;
            }

            _context.TopSearchResults.RemoveRange(topSearchResultEntities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetTopSearchResultsUserList(CancellationToken cancellationToken = default)
        {
            return await _context.TopSearchResults.Select(tsr => tsr.UserId).Distinct().ToListAsync(cancellationToken);
        }

        public async Task<int> DeleteTopSearchResultsFromDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var resultsToDelete = await _context.TopSearchResults
                .Where(r => r.CreatedOn > date)
                .ToListAsync(cancellationToken);

            if (resultsToDelete.Any())
            {
                _context.TopSearchResults.RemoveRange(resultsToDelete);
                return await _context.SaveChangesAsync(cancellationToken);
            }

            return 0;
        }

        public async Task<int> DeleteTopSearchResultsBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var resultsToDelete = await _context.TopSearchResults
                .Where(r => r.CreatedOn < date)
                .ToListAsync(cancellationToken);

            if (resultsToDelete.Any())
            {
                _context.TopSearchResults.RemoveRange(resultsToDelete);
                return await _context.SaveChangesAsync(cancellationToken);
            }

            return 0;
        }
    }
}