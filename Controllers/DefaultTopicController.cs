using AutoMapper;
using DopamineDetox.Domain.Dtos;
using DopamineDetox.Domain.Models;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DefaultTopicController : Controller
    {
        private readonly IDefaultTopicService _topicService;
        private readonly IMapper _mapper;
        private readonly ILoggingService _logger;

        public DefaultTopicController(IDefaultTopicService topicService, IMapper mapper, ILoggingService logger)
        {
            _topicService = topicService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<DefaultTopicDto>>> GetDefaultTopics(CancellationToken cancellationToken = default)
        {
            var topics = _mapper.Map<List<DefaultTopicDto>>(await _topicService.GetDefaultTopicsAsync());

            if (topics == null)
            {
                return NotFound();
            }

            return topics;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDto>> GetDefaultTopic(int id, CancellationToken cancellationToken = default)
        {
            var topic = _mapper.Map<TopicDto>(await _topicService.GetDefaultTopicAsync(id));

            if (topic == null)
            {
                return NotFound();
            }

            return topic;
        }

        [HttpPost]
        public async Task<ActionResult<DefaultTopicDto>> PostDefaultTopic(DefaultTopicDto topic, CancellationToken cancellationToken = default)
        {
            var createdTopic = await _topicService.CreateDefaultTopicAsync(_mapper.Map<DefaultTopic>(topic));

            return CreatedAtAction(nameof(GetDefaultTopic), new { id = createdTopic.Id }, _mapper.Map<DefaultTopicDto>(createdTopic));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDefaultTopic(int id, DefaultTopicDto topic, CancellationToken cancellationToken = default)
        {
            if (id != topic.Id)
            {
                return BadRequest();
            }

            await _topicService.UpdateDefaultTopicAsync(id, _mapper.Map<DefaultTopic>(topic));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDefaultTopic(int id, CancellationToken cancellationToken = default)
        {
            await _topicService.DeleteDefaultTopicAsync(id);

            return NoContent();
        }
    }
}
