using DopamineDetox.Domain.Dtos;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoggingController : Controller
    {
        private readonly ILoggingService _loggingService;

        public LoggingController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        [HttpPost]
        public async Task<ActionResult> LogError(WebTraceDto loggingDto, CancellationToken cancellationToken = default)
        {
            await _loggingService.LogErrorAsync(loggingDto.ErrorCode, loggingDto.ErrorMessage, loggingDto.CallStack);
            return NoContent();
        }


        [HttpPost("all")]
        public async Task<ActionResult> LogAllErrors([FromBody] Dictionary<string, IEnumerable<WebTraceDto>> payload, CancellationToken cancellationToken = default)
        {
            if (payload != null && payload.TryGetValue("request", out var request) && request.Any())
            {
                foreach (var loggingDto in request)
                {
                    await _loggingService.LogErrorAsync(loggingDto.ErrorCode, loggingDto.ErrorMessage, loggingDto.CallStack);
                }
            }
            return NoContent();
        }
    }
}
