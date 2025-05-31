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
        private readonly IHealthService _healthService;

        public HealthController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        // Test automatycznego wdrażania GitHub Actions
        [HttpPost("assess-advanced")]
        public async Task<IActionResult> AssessAdvancedHealth([FromBody] HealthRequest request)
        {
            try
            {
                var result = await _healthService.AssessAdvancedHealthAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Błąd podczas zaawansowanej analizy zdrowotnej", details = ex.Message });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        [HttpPost("assess")]
        public async Task<ActionResult<HealthResponse>> Assess(
            [FromBody] HealthRequest req)
        {
            var result = await _healthService.AssessAsync(req);
            return Ok(result);
        }

        [HttpPost("assess-simple")]
        public async Task<ActionResult<HealthResponse>> AssessSimple(
            [FromBody] SimpleHealthRequest req)
        {
            var result = await _healthService.AssessSimpleAsync(req);
            return Ok(result);
        }
    }
}
