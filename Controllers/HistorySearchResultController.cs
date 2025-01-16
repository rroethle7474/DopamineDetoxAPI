using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistorySearchResultController : Controller
    {
        private readonly IHistorySearchResultService _historySearchResultService;
        private readonly IMapper _mapper;

        public HistorySearchResultController(IHistorySearchResultService historySearchResultService, IMapper mapper)
        {
            _historySearchResultService = historySearchResultService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistorySearchResultDto>>> GetHistorySearchResults(CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<HistorySearchResultDto>>(await _historySearchResultService.GetHistorySearchResultsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistorySearchResultDto>> GetHistorySearchResult(int id, CancellationToken cancellationToken = default)
        {
            var historySearchResult = await _historySearchResultService.GetHistorySearchResultAsync(id);

            if (historySearchResult == null)
            {
                return NotFound();
            }

            return _mapper.Map<HistorySearchResultDto>(historySearchResult);
        }

        [HttpPost]
        public async Task<ActionResult<HistorySearchResultDto>> CreateHistorySearchResult(HistorySearchResultDto historySearchResultDto, CancellationToken cancellationToken = default)
        {
            var createdHistorySearchResult = await _historySearchResultService.CreateHistorySearchResultAsync(_mapper.Map<HistorySearchResult>(historySearchResultDto));

            return CreatedAtAction(nameof(GetHistorySearchResult), new { id = createdHistorySearchResult.Id }, _mapper.Map<HistorySearchResultDto>(createdHistorySearchResult));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditHistorySearchResult(int id, HistorySearchResultDto historySearchResultDto, CancellationToken cancellationToken = default)
        {
            if (id != historySearchResultDto.Id)
            {
                return BadRequest();
            }

            await _historySearchResultService.UpdateHistorySearchResultAsync(id, _mapper.Map<HistorySearchResult>(historySearchResultDto));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHistorySearchResult(int id, CancellationToken cancellationToken = default)
        {
            await _historySearchResultService.DeleteHistorySearchResultAsync(id);

            return NoContent();
        }
    }
}
