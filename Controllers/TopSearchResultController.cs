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
    public class TopSearchResultController : Controller
    {
        private readonly ITopSearchResultService _topSearchResultService;
        private readonly IMapper _mapper;

        public TopSearchResultController(ITopSearchResultService topSearchResultService, IMapper mapper)
        {
            _topSearchResultService = topSearchResultService;
            _mapper = mapper;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<TopSearchResultDto>>> GetTopSearchResults(GetTopSearchResultsRequest request, CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<TopSearchResultDto>>(await _topSearchResultService.GetTopSearchResultsAsync(request));
        }

        [HttpGet("user-list")]
        public async Task<ActionResult<IEnumerable<string>>> GetTopSearchResultsUserList(CancellationToken cancellationToken = default)
        {
            var results = await _topSearchResultService.GetTopSearchResultsUserList(cancellationToken);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TopSearchResultDto>> GetTopSearchResult(int id, CancellationToken cancellationToken = default)
        {
            var topSearchResult = await _topSearchResultService.GetTopSearchResultAsync(id, cancellationToken);

            if (topSearchResult == null)
            {
                return NotFound();
            }

            return _mapper.Map<TopSearchResultDto>(topSearchResult);
        }

        [HttpPost]
        public async Task<ActionResult<TopSearchResultDto>> CreateTopSearchResult(TopSearchResultDto topSearchResultDto, CancellationToken cancellationToken = default)
        {
            var createdTopSearchResult = await _topSearchResultService.CreateTopSearchResultAsync(_mapper.Map<TopSearchResult>(topSearchResultDto));

            return CreatedAtAction(nameof(GetTopSearchResult), new { id = createdTopSearchResult.Id }, _mapper.Map<TopSearchResultDto>(createdTopSearchResult));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditTopSearchResult(int id, TopSearchResultDto topSearchResultDto, CancellationToken cancellationToken = default)
        {
            if (id != topSearchResultDto.Id)
            {
                return BadRequest();
            }

            await _topSearchResultService.UpdateTopSearchResultAsync(id, _mapper.Map<TopSearchResult>(topSearchResultDto));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopSearchResult(int id, CancellationToken cancellationToken = default)
        {
            await _topSearchResultService.DeleteTopSearchResultAsync(id);

            return NoContent();
        }

        [HttpDelete("deleteByDate")]
        public async Task<IActionResult> DeleteTopSearchResultsByDate([FromQuery] DateTime date, [FromQuery] bool isBefore = false, CancellationToken cancellationToken = default)
        {
            int deletedCount = 0;
            if(isBefore)
            {
                deletedCount = await _topSearchResultService.DeleteTopSearchResultsBeforeDateAsync(date, cancellationToken);
            }
            else
            {
                deletedCount = await _topSearchResultService.DeleteTopSearchResultsFromDateAsync(date, cancellationToken);
            }

            if (deletedCount > 0)
            {
                return Ok(new { message = $"{deletedCount} top search results deleted." });
            }

            return NotFound(new { message = "No top search results found to delete." });
        }

    }
}
