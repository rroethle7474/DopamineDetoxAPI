using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class SearchResultReportService : ISearchResultReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SearchResultReportService(ApplicationDbContext context, IMapper mapper, CacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SearchResultReport> CreateSearchResultReportAsync(SearchResultReport searchResultReport, CancellationToken cancellationToken = default)
        {
            var searchResultReportEntity = _mapper.Map<SearchResultReportEntity>(searchResultReport);

            _context.SearchResultReports.Add(searchResultReportEntity);
            await _context.SaveChangesAsync(cancellationToken);

            searchResultReport.Id = searchResultReportEntity.Id;

            return searchResultReport;
        }

        public async Task<SearchResultReport> GetSearchResultReportAsync(int id, CancellationToken cancellationToken = default)
        {
            var searchResultReportEntity = await _context.SearchResultReports.FindAsync(new object[] { id }, cancellationToken);

            if (searchResultReportEntity == null)
            {
                throw new Exception($"SearchResultReport with id {id} not found");
            }

            return _mapper.Map<SearchResultReport>(searchResultReportEntity);
        }

        public async Task<IEnumerable<SearchResultReport>> GetSearchResultReportsAsync(GetSearchResultReportRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new Exception($"Null request found");
            }

            var fromDate = DateTime.Today;
            var toDate = DateTime.Today.AddDays(1);

            var query = _context.SearchResultReports.AsQueryable();

            if (request.Id.HasValue)
            {
                query = query.Where(r => r.Id == request.Id.Value);
            }

            if (request.IsSuccess.HasValue)
            {
                query = query.Where(r => r.IsSuccess == request.IsSuccess.Value);
            }
            else
            {
                query = query.Where(r => r.IsSuccess == true);
            }

            if (request.ContentTypeId != null && request.ContentTypeId > 0)
            {
                query = query.Where(r => r.ContentTypeId == request.ContentTypeId);
            }

            if (request.IsDefaultReport == false)
            {
                query = query.Where(r => r.IsDefaultReport == false);
            }
            else if(request.IsDefaultReport == true)
            {
                query = query.Where(r => r.IsDefaultReport == true);
            }

            if (request.IsChannelReport == false)
            {
                query = query.Where(r => r.IsChannelReport == false);
            }
            else if (request.IsChannelReport == true)
            {
                query = query.Where(r => r.IsChannelReport == true);
            }

            query = query.Where(r => r.ReportDate >= fromDate && r.ReportDate < toDate);

            return _mapper.Map<List<SearchResultReport>>(await query.ToListAsync(cancellationToken));
        }

        public async Task<SearchResultReport> UpdateSearchResultReportAsync(int id, SearchResultReport searchResultReport, CancellationToken cancellationToken = default)
        {
            var searchResultReportEntity = await _context.SearchResultReports.FindAsync(new object[] { id }, cancellationToken);

            if (searchResultReportEntity == null)
            {
                throw new Exception($"SearchResultReport with id {id} not found");
            }



            _mapper.Map(searchResultReport, searchResultReportEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return searchResultReport;
        }

        public async Task DeleteSearchResultReportAsync(int id, CancellationToken cancellationToken = default)
        {
            var searchResultReportEntity = await _context.SearchResultReports.FindAsync(new object[] { id }, cancellationToken);

            if (searchResultReportEntity == null)
            {
                throw new Exception($"SearchResultReport with id {id} not found");
            }

            _context.SearchResultReports.Remove(searchResultReportEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> DeleteSearchResultReportsFromDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var reportsToDelete = await _context.SearchResultReports
                .Where(r => r.ReportDate > date)
                .ToListAsync(cancellationToken);

            if (reportsToDelete.Any())
            {
                _context.SearchResultReports.RemoveRange(reportsToDelete);
                return await _context.SaveChangesAsync(cancellationToken);
            }

            return 0;
        }

        public async Task<int> DeleteSearchResultReportsBeforeDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var reportsToDelete = await _context.SearchResultReports
                .Where(r => r.ReportDate < date)
                .ToListAsync(cancellationToken);

            if (reportsToDelete.Any())
            {
                _context.SearchResultReports.RemoveRange(reportsToDelete);
                return await _context.SaveChangesAsync(cancellationToken);
            }

            return 0;
        }
    }
}