using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class HistorySearchResultService : IHistorySearchResultService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HistorySearchResultService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<HistorySearchResult> CreateHistorySearchResultAsync(HistorySearchResult historySearchResult, CancellationToken cancellationToken = default)
        {
            var historySearchResultEntity = _mapper.Map<HistorySearchResultEntity>(historySearchResult);

            _context.HistorySearchResults.Add(historySearchResultEntity);
            await _context.SaveChangesAsync(cancellationToken);

            historySearchResult.Id = historySearchResultEntity.Id;

            return historySearchResult;
        }

        public async Task<HistorySearchResult> GetHistorySearchResultAsync(int id, CancellationToken cancellationToken = default)
        {
            var historySearchResultEntity = await _context.HistorySearchResults.FindAsync(id, cancellationToken);

            if (historySearchResultEntity == null)
            {
                throw new Exception($"HistorySearchResult with id {id} not found");
            }

            return _mapper.Map<HistorySearchResult>(historySearchResultEntity);
        }

        public async Task<IEnumerable<HistorySearchResult>> GetHistorySearchResultsAsync(CancellationToken cancellationToken = default)
        {
            var historySearchResultEntities = await _context.HistorySearchResults.ToListAsync(cancellationToken);
            return _mapper.Map<IEnumerable<HistorySearchResult>>(historySearchResultEntities);
        }

        public async Task<HistorySearchResult> UpdateHistorySearchResultAsync(int id, HistorySearchResult historySearchResult, CancellationToken cancellationToken = default)
        {
            var historySearchResultEntity = await _context.HistorySearchResults.FindAsync(id, cancellationToken);

            if (historySearchResultEntity == null)
            {
                throw new Exception($"HistorySearchResult with id {id} not found");
            }

            _mapper.Map(historySearchResult, historySearchResultEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return historySearchResult;
        }

        public async Task DeleteHistorySearchResultAsync(int id, CancellationToken cancellationToken = default)
        {
            var historySearchResultEntity = await _context.HistorySearchResults.FindAsync(id, cancellationToken);

            if (historySearchResultEntity == null)
            {
                throw new Exception($"HistorySearchResult with id {id} not found");
            }

            _context.HistorySearchResults.Remove(historySearchResultEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
