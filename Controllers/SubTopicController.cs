using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Models.Entities;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubTopicController : Controller
    {
        private readonly ISubTopicService _subTopicService;
        private readonly IMapper _mapper;

        public SubTopicController(ISubTopicService subTopicService, IMapper mapper)
        {
            _subTopicService = subTopicService;
            _mapper = mapper;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<SubTopicDto>>> GetSubTopics([FromBody] GetSubTopicsRequest request, CancellationToken cancellationToken = default)
        {
            var subTopics = await _subTopicService.GetSubTopicsAsync(request);
            return _mapper.Map<List<SubTopicDto>>(subTopics);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SubTopicDto>> GetSubTopic(int id, CancellationToken cancellationToken = default)
        {
            var subTopic = await _subTopicService.GetSubTopicAsync(id);

            if (subTopic == null)
            {
                return NotFound();
            }

            return _mapper.Map<SubTopicDto>(subTopic);
        }

        [HttpPost]
        public async Task<ActionResult<SubTopicEntity>> CreateSubTopic(SubTopicDto subTopic, CancellationToken cancellationToken = default)
        {
            bool isValidRequest = _subTopicService.ValidateCreateSubTopicDto(subTopic);

            if (!isValidRequest)
            {
                return BadRequest("UserId, Term, and TopicId are required fields.");
            }

            var existingSubTopic = await _subTopicService.GetSubTopicByUserIdAndTermAsync(subTopic.UserId, subTopic.Term);
            if (existingSubTopic != null)
            {
                return BadRequest($"The term '{subTopic.Term}' is already associated with another subtopic.");
            }

            var createdSubTopic = await _subTopicService.CreateSubTopicAsync(_mapper.Map<SubTopic>(subTopic));

            return CreatedAtAction(nameof(GetSubTopic), new { id = createdSubTopic.Id }, _mapper.Map<SubTopicDto>(createdSubTopic));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditSubTopic(int id, SubTopicDto subTopic, CancellationToken cancellationToken = default)
        {
            if (id != subTopic.Id || String.IsNullOrEmpty(subTopic?.UserId) || String.IsNullOrEmpty(subTopic?.Term))
            {
                return BadRequest();
            }

            var existingSubTopic = await _subTopicService.GetSubTopicByUserIdAndTermAsync(subTopic.UserId, subTopic.Term);
            if (existingSubTopic != null && existingSubTopic.Id != id)
            {
                return BadRequest($"The term '{subTopic.Term}' is already associated with another subtopic.");
            }

            await _subTopicService.UpdateSubTopicAsync(id, _mapper.Map<SubTopic>(subTopic));

            return NoContent();
        }

        [HttpPatch("{id}/inactivate")]
        public async Task<IActionResult> InactivateSubTopic(int id, CancellationToken cancellationToken = default)
        {
            var subTopic = await _subTopicService.GetSubTopicAsync(id);
            if (subTopic == null)
            {
                return NotFound();
            }

            subTopic.IsActive = false;
            await _subTopicService.UpdateSubTopicAsync(id, subTopic);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubTopic(int id, CancellationToken cancellationToken = default)
        {
            await _subTopicService.DeleteSubTopicAsync(id);

            return NoContent();
        }
    }
}
