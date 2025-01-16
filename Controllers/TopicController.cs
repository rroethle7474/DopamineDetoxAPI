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
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly IMapper _mapper;
        private readonly ILoggingService _logger;

        public TopicController(ITopicService topicService, IMapper mapper, ILoggingService logger)
        {
            _topicService = topicService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<TopicDto>>> GetTopics([FromBody] GetTopicsRequest request, CancellationToken cancellationToken = default)
        {
            var topics = new List<TopicDto>();
            try
            {
                topics = _mapper.Map<List<TopicDto>>(await _topicService.GetTopicsAsync(request));
            }
            catch (Exception e)
            {
                await _logger.LogErrorAsync("GetTopicsError", e.Message, e.StackTrace);
            }
            return topics;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDto>> GetTopic(int id, CancellationToken cancellationToken = default)
        {
            var topic = _mapper.Map<TopicDto>(await _topicService.GetTopicAsync(id));

            if (topic == null)
            {
                return NotFound();
            }

            return topic;
        }

        [HttpPost]
        public async Task<ActionResult<TopicDto>> PostTopic(TopicDto topic, CancellationToken cancellationToken = default)
        {
            var createdTopic = await _topicService.CreateTopicAsync(_mapper.Map<Topic>(topic));

            return CreatedAtAction(nameof(GetTopic), new { id = createdTopic.Id }, _mapper.Map<TopicDto>(createdTopic));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTopic(int id, TopicDto topic, CancellationToken cancellationToken = default)
        {
            if (id != topic.Id)
            {
                return BadRequest();
            }

            await _topicService.UpdateTopicAsync(id, _mapper.Map<Topic>(topic));

            return NoContent();
        }

        [HttpPatch("{id}/inactivate")]
        public async Task<IActionResult> InactivateTopic(int id, CancellationToken cancellationToken = default)
        {
            var topic = await _topicService.GetTopicAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            topic.IsActive = false;
            await _topicService.UpdateTopicAsync(id, topic);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id, CancellationToken cancellationToken = default)
        {
            await _topicService.DeleteTopicAsync(id);

            return NoContent();
        }
    }
}
