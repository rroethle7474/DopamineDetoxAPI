using DopamineDetox.ServiceAgent.Requests;
using DopamineDetoxAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : Controller
    {
        //private readonly ICloudEmailService _emailService;
        private readonly INotificationService _notificationService;

        public EmailController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send")]
        public async Task<ActionResult<bool>> Send(SendEmailRequest request, CancellationToken cancellationToken = default)
        {
            if(request == null)
            {
                return BadRequest("No email request provided.");
            }

            return Ok("true");
        }
    }
}
