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
        private readonly AdvancedHealthAnalyzer _analyzer;
        
        public HealthController(IHealthService svc, AdvancedHealthAnalyzer analyzer)
        {
            _svc = svc;
            _analyzer = analyzer;
        }

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

        [HttpPost("assess-advanced")]
        public async Task<ActionResult<AdvancedHealthAnalysisResult>> AssessAdvanced(
            [FromBody] HealthRequest req)
        {
            try
            {
                // Najpierw pobierz podstawową predykcję
                var basicResult = await _svc.AssessAsync(req);
                
                // Następnie wykonaj zaawansowaną analizę
                var advancedResult = _analyzer.AnalyzeHealth(req, basicResult.Assessments);
                
                return Ok(advancedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Błąd podczas zaawansowanej analizy zdrowotnej", 
                    details = ex.Message 
                });
            }
        }
    }
}
