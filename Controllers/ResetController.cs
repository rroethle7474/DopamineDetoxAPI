using DopamineDetoxAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DopamineDetoxAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResetController : Controller
    {
        private readonly CacheService _cacheService;

        public ResetController(CacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet("clear-cache")]
        public ActionResult<bool> ClearCache()
        {
            _cacheService.ClearAll();
            return Ok(true);
        }
    }
}
