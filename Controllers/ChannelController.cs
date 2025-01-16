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
    public class ChannelController : Controller
    {
        private readonly IChannelService _channelService;
        private readonly IMapper _mapper;

        public ChannelController(IChannelService channelService, IMapper mapper)
        {
            _channelService = channelService;
            _mapper = mapper;
        }

        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<ChannelDto>>> GetChannels([FromBody] GetChannelsRequest request, CancellationToken cancellationToken = default)
        {
            var channels = await _channelService.GetChannelsAsync(request, cancellationToken);
            var channelDtos = _mapper.Map<IEnumerable<ChannelDto>>(channels);
            return Ok(channelDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChannelDto>> GetChannel(int id, CancellationToken cancellationToken = default)
        {
            var channel = await _channelService.GetChannelAsync(id, cancellationToken);
            if (channel == null)
            {
                return NotFound();
            }
            var channelDto = _mapper.Map<ChannelDto>(channel);
            return Ok(channelDto);
        }

        [HttpPost]
        public async Task<ActionResult<ChannelDto>> CreateChannel(ChannelDto channelDto, CancellationToken cancellationToken = default)
        {
            var channel = _mapper.Map<Channel>(channelDto);
            var createdChannel = await _channelService.CreateChannelAsync(channel, cancellationToken);
            var createdChannelDto = _mapper.Map<ChannelDto>(createdChannel);
            return CreatedAtAction(nameof(GetChannel), new { id = createdChannelDto.Id }, createdChannelDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditChannel(int id, ChannelDto channelDto, CancellationToken cancellationToken = default)
        {
            var channel = _mapper.Map<Channel>(channelDto);
            var updatedChannel = await _channelService.UpdateChannelAsync(id, channel, cancellationToken);
            if (updatedChannel == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChannel(int id, CancellationToken cancellationToken = default)
        {
            await _channelService.DeleteChannelAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
