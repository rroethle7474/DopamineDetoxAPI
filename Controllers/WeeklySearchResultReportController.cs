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
    public class WeeklySearchResultReportController : Controller
    {
        private readonly IWeeklySearchResultReportService _weeklySearchResultReportService;
        private readonly ISearchResultService _searchResultService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ILoggingService _logger;
        private readonly INotificationService _notificationService;

        public WeeklySearchResultReportController(IWeeklySearchResultReportService weeklySearchResultReportService, ISearchResultService searchResultService, IUserService userService, IMapper mapper, ILoggingService logger, INotificationService notificationService)
        {
            _weeklySearchResultReportService = weeklySearchResultReportService;
            _searchResultService = searchResultService;
            _notificationService = notificationService;
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<WeeklySearchResultReportDto>>> GetWeeklySearchResultReports([FromBody] GetWeeklySearchResultReportsRequest request, CancellationToken cancellationToken = default)
        {
            var results = new List<WeeklySearchResultReportDto>();
            try
            {
                results = _mapper.Map<List<WeeklySearchResultReportDto>>(await _weeklySearchResultReportService.GetWeeklySearchResultReportsAsync(request, cancellationToken));
            }
            catch (Exception e)
            {
                await _logger.LogErrorAsync("Get Weekly Search Result Report Error", e.Message, e.StackTrace);
            }
            return results;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WeeklySearchResultReportDto>> GetWeeklySearchResultReport(int id, CancellationToken cancellationToken = default)
        {
            var weeklySearchResultReport = await _weeklySearchResultReportService.GetWeeklySearchResultReportAsync(id, cancellationToken);

            if (weeklySearchResultReport == null)
            {
                return NotFound();
            }

            return _mapper.Map<WeeklySearchResultReportDto>(weeklySearchResultReport);
        }

        [HttpPost]
        public async Task<ActionResult<WeeklySearchResultReportDto>> CreateWeeklySearchResultReport(WeeklySearchResultReportDto weeklySearchResultReportDto, CancellationToken cancellationToken = default)
        {
            var createdWeeklySearchResultReport = await _weeklySearchResultReportService.CreateWeeklySearchResultReportAsync(_mapper.Map<WeeklySearchResultReport>(weeklySearchResultReportDto));

            return CreatedAtAction(nameof(GetWeeklySearchResultReport), new { id = createdWeeklySearchResultReport.Id }, _mapper.Map<WeeklySearchResultReportDto>(createdWeeklySearchResultReport));
        }

        [HttpPost("email/{userId}")]
        public async Task<ActionResult<bool>> EmailUserWeeklyReport(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await _userService.GetUserById(userId);

                if(user == null)
                {
                    await _logger.LogErrorAsync("User Not Found", $"{userId} was not found", $"{userId} was not found");
                    return false;
                }

                var mvpSearchResults = await _searchResultService.GetSearchResultsAsync(new GetSearchResultsRequest { UserId = userId, isMVPResults = true }, cancellationToken);

                //if(mvpSearchResults == null || !mvpSearchResults.Any())
                //{
                //    await _logger.LogErrorAsync("NULL MVP SEARCH RESULTS", $"{userId} did not return any MVP results", $"{userId} did not return any MVP results");
                //    return false;
                //}

                try
                {
                    bool isSuccess = await _notificationService.SendMVPWeeklyEmail(user, new List<SearchResult>(), cancellationToken);
                    
                    if (isSuccess)
                    {
                        return Ok(true);
                    }
                    else
                    {
                        return BadRequest("Email failed to send.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                return true;
            }
            catch(Exception e)
            {
                await _logger.LogErrorAsync("Error Emailing User Weekly Report", e.Message, e.StackTrace);
                return false;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditWeeklySearchResultReport(int id, WeeklySearchResultReportDto weeklySearchResultReportDto, CancellationToken cancellationToken = default)
        {
            if (id != weeklySearchResultReportDto.Id)
            {
                return BadRequest();
            }

            await _weeklySearchResultReportService.UpdateWeeklySearchResultReportAsync(id, _mapper.Map<WeeklySearchResultReport>(weeklySearchResultReportDto));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeeklySearchResultReport(int id, CancellationToken cancellationToken = default)
        {
            await _weeklySearchResultReportService.DeleteWeeklySearchResultReportAsync(id);

            return NoContent();
        }
    }
}
