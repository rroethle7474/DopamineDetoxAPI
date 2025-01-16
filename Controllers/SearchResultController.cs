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
    public class SearchResultController : Controller
    {
        private readonly ISearchResultService _searchResultService;
        private readonly IMapper _mapper;

        public SearchResultController(ISearchResultService searchResultService, IMapper mapper)
        {
            _searchResultService = searchResultService;
            _mapper = mapper;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<SearchResultDto>>> GetSearchResults([FromBody] GetSearchResultsRequest request, CancellationToken cancellationToken = default)
        {
            var searchResults = await _searchResultService.GetSearchResultsAsync(request);
            var test = _mapper.Map<List<SearchResultDto>>(searchResults);
            return Ok(_mapper.Map<List<SearchResultDto>>(searchResults));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SearchResultDto>> GetSearchResult(int id, CancellationToken cancellationToken = default)
        {
            var searchResult = await _searchResultService.GetSearchResultAsync(id);

            if (searchResult == null)
            {
                return NotFound();
            }

            return _mapper.Map<SearchResultDto>(searchResult);
        }

        [HttpPost]
        public async Task<ActionResult<SearchResultDto>> CreateSearchResult(SearchResultDto searchResultDto, CancellationToken cancellationToken = default)
        {
            var createdSearchResult = await _searchResultService.CreateSearchResultAsync(_mapper.Map<SearchResult>(searchResultDto));

            return CreatedAtAction(nameof(GetSearchResult), new { id = createdSearchResult.Id }, _mapper.Map<SearchResultDto>(createdSearchResult));
        }

        [HttpPost("AddMultipleSearchResults")]
        public async Task<ActionResult<SearchResultsResponseDto>> AddMultipleSearchResultsAsync([FromBody] IEnumerable<SearchResultDto> searchResultsDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (searchResultsDto == null || !searchResultsDto.Any())
                {
                    return BadRequest("No search results provided.");
                }

                var result = await _searchResultService.AddMultipleSearchResultsAsync(_mapper.Map<List<SearchResult>>(searchResultsDto));

                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500, $"An error occurred while adding search results. {e.Message}");
            }   
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSearchResult(int id, SearchResultDto searchResultDto, CancellationToken cancellationToken = default)
        {
            if (id != searchResultDto.Id)
            {
                return BadRequest();
            }

            await _searchResultService.UpdateSearchResultAsync(id, _mapper.Map<SearchResult>(searchResultDto));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSearchResult(int id, CancellationToken cancellationToken = default)
        {
            await _searchResultService.DeleteSearchResultAsync(id);

            return NoContent();
        }

        [HttpDelete("deleteByDate")]
        public async Task<IActionResult> DeleteSearchResultsByDate([FromQuery] DateTime date, [FromQuery]bool isBefore, CancellationToken cancellationToken)
        {
            int deletedCount = 0;

            if(isBefore)
            {
                deletedCount = await _searchResultService.DeleteSearchResultsBeforeDateAsync(date, cancellationToken);
            }
            else
            {
                deletedCount = await _searchResultService.DeleteSearchResultsFromDateAsync(date, cancellationToken);
            }

            if (deletedCount > 0)
            {
                return Ok(new { message = $"{deletedCount} search results deleted." });
            }

            return NotFound(new { message = "No search results found to delete." });
        }
    }
}
