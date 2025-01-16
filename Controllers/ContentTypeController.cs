using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentTypeController : ControllerBase
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IMapper _mapper;

        public ContentTypeController(IContentTypeService contentTypeService, IMapper mapper)
        {
            _contentTypeService = contentTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentTypeDto>>> GetContentTypes(CancellationToken cancellationToken = default)
        {
            var contentTypes = await _contentTypeService.GetContentTypesAsync(); 
            return _mapper.Map<List<ContentTypeDto>>(contentTypes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentTypeDto>> GetContentType(int id, CancellationToken cancellationToken = default)
        {
            var contentType = await _contentTypeService.GetContentTypeAsync(id);

            if (contentType == null)
            {
                return NotFound();
            }

            return _mapper.Map<ContentTypeDto>(contentType);
        }

        [HttpPost]
        public async Task<ActionResult<ContentTypeDto>> CreateContentType(ContentTypeDto contentType, CancellationToken cancellationToken = default)
        {
            var createdContentType = await _contentTypeService.CreateContentTypeAsync(_mapper.Map<ContentType>(contentType));

            return CreatedAtAction(nameof(GetContentType), new { id = createdContentType.Id }, _mapper.Map<ContentTypeDto>(createdContentType));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditContentType(int id, ContentTypeDto contentType, CancellationToken cancellationToken = default)
        {
            if (id != contentType.Id)
            {
                return BadRequest();
            }

            await _contentTypeService.UpdateContentTypeAsync(id, _mapper.Map<ContentType>(contentType));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContentType(int id, CancellationToken cancellationToken = default)
        {
            await _contentTypeService.DeleteContentTypeAsync(id);

            return NoContent();
        }
    }
}
