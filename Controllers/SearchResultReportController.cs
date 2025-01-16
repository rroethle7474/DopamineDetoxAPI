using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchResultReportController : Controller
    {
        private readonly ISearchResultReportService _searchResultReportService;
        private readonly IMapper _mapper;
        private readonly ILoggingService _logger;

        public SearchResultReportController(ISearchResultReportService searchResultReportService, IMapper mapper, ILoggingService logger)
        {
            _searchResultReportService = searchResultReportService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SearchResultReportDto>> GetSearchResultReport(int id, CancellationToken cancellationToken = default)
        {
            var searchResultReport = await _searchResultReportService.GetSearchResultReportAsync(id);

            if (searchResultReport == null)
            {
                return NotFound();
            }

            return _mapper.Map<SearchResultReportDto>(searchResultReport);
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<SearchResultReportDto>>> GetSearchResultReports([FromBody] GetSearchResultReportRequest request, CancellationToken cancellationToken = default)
        {
            var results = new List<SearchResultReportDto>();
            try
            {
                results = _mapper.Map<List<SearchResultReportDto>>(await _searchResultReportService.GetSearchResultReportsAsync(request));
            }
            catch (Exception e)
            {
                await _logger.LogErrorAsync("Get Search Result Report Error", e.Message, e.StackTrace);
            }
            return results;
        }

        [HttpPost]
        public async Task<ActionResult<SearchResultReportDto>> CreateSearchResultReport(SearchResultReportDto searchResultReportDto, CancellationToken cancellationToken = default)
        {
            var createdSearchResultReport = await _searchResultReportService.CreateSearchResultReportAsync(_mapper.Map<SearchResultReport>(searchResultReportDto));

            return CreatedAtAction(nameof(GetSearchResultReport), new { id = createdSearchResultReport.Id }, _mapper.Map<SearchResultReportDto>(createdSearchResultReport));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSearchResultReport(int id, SearchResultReportDto searchResultReportDto, CancellationToken cancellationToken = default)
        {
            if (id != searchResultReportDto.Id)
            {
                return BadRequest();
            }

            await _searchResultReportService.UpdateSearchResultReportAsync(id, _mapper.Map<SearchResultReport>(searchResultReportDto));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSearchResultReport(int id, CancellationToken cancellationToken = default)
        {
            await _searchResultReportService.DeleteSearchResultReportAsync(id);

            return NoContent();
        }

        [HttpDelete("deleteByDate")]
        public async Task<IActionResult> DeleteSearchResultReportsByDate([FromQuery] DateTime date, [FromQuery] bool isBefore, CancellationToken cancellationToken)
        {
            int deletedCount = 0;
            if(isBefore)
            {
                deletedCount = await _searchResultReportService.DeleteSearchResultReportsBeforeDateAsync(date, cancellationToken);
            }
            else
            {
                deletedCount = await _searchResultReportService.DeleteSearchResultReportsFromDateAsync(date, cancellationToken);
            };

            if (deletedCount > 0)
            {
                return Ok(new { message = $"{deletedCount} search result reports deleted." });
            }

            return NotFound(new { message = "No search result reports found to delete." });
        }
    }
}
