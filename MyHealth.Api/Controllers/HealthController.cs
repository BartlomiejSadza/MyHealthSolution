using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyHealth.Api.Models;
using MyHealth.Api.Services;

namespace MyHealth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IHealthService _svc;
        public HealthController(IHealthService svc) => _svc = svc;

        [HttpGet]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        [HttpPost("assess")]
        public async Task<ActionResult<HealthResponse>> Assess(
            [FromBody] HealthRequest req)
        {
            var result = await _svc.AssessAsync(req);
            return Ok(result);
        }

        [HttpPost("assess-simple")]
        public async Task<ActionResult<HealthResponse>> AssessSimple(
            [FromBody] SimpleHealthRequest req)
        {
            var result = await _svc.AssessSimpleAsync(req);
            return Ok(result);
        }
    }
}
