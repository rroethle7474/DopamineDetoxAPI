using AutoMapper;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Data;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DopamineDetoxAPI.Services
{
    public class WeeklySearchResultReportService : IWeeklySearchResultReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public WeeklySearchResultReportService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<WeeklySearchResultReport> CreateWeeklySearchResultReportAsync(WeeklySearchResultReport weeklySearchResultReport, CancellationToken cancellationToken = default)
        {
            var weeklySearchResultReportEntity = _mapper.Map<WeeklySearchResultReportEntity>(weeklySearchResultReport);

            _context.WeeklySearchResultReports.Add(weeklySearchResultReportEntity);
            await _context.SaveChangesAsync(cancellationToken);

            weeklySearchResultReport.Id = weeklySearchResultReportEntity.Id;

            return weeklySearchResultReport;
        }

        public async Task<WeeklySearchResultReport> GetWeeklySearchResultReportAsync(int id, CancellationToken cancellationToken = default)
        {
            var weeklySearchResultReportEntity = await _context.WeeklySearchResultReports.FindAsync(new object[] { id }, cancellationToken);

            if (weeklySearchResultReportEntity == null)
            {
                throw new Exception($"WeeklySearchResultReport with id {id} not found");
            }

            return _mapper.Map<WeeklySearchResultReport>(weeklySearchResultReportEntity);
        }

        public async Task<IEnumerable<WeeklySearchResultReport>> GetWeeklySearchResultReportsAsync(GetWeeklySearchResultReportsRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new Exception($"Null request found");
            }

            var fromDate = DateTime.Today.AddDays(-8);
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

            query = query.Where(r => r.ReportDate >= fromDate && r.ReportDate < toDate);

            return _mapper.Map<List<WeeklySearchResultReport>>(await query.ToListAsync(cancellationToken));
        }

        public async Task<WeeklySearchResultReport> UpdateWeeklySearchResultReportAsync(int id, WeeklySearchResultReport weeklySearchResultReport, CancellationToken cancellationToken = default)
        {
            var weeklySearchResultReportEntity = await _context.WeeklySearchResultReports.FindAsync(new object[] { id }, cancellationToken);

            if (weeklySearchResultReportEntity == null)
            {
                throw new Exception($"WeeklySearchResultReport with id {id} not found");
            }

            _mapper.Map(weeklySearchResultReport, weeklySearchResultReportEntity);

            await _context.SaveChangesAsync(cancellationToken);

            return weeklySearchResultReport;
        }

        public async Task DeleteWeeklySearchResultReportAsync(int id, CancellationToken cancellationToken = default)
        {
            var weeklySearchResultReportEntity = await _context.WeeklySearchResultReports.FindAsync(new object[] { id }, cancellationToken);

            if (weeklySearchResultReportEntity == null)
            {
                throw new Exception($"WeeklySearchResultReport with id {id} not found");
            }

            _context.WeeklySearchResultReports.Remove(weeklySearchResultReportEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}