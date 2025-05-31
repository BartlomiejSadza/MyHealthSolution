using System.Threading.Tasks;
using MyHealth.Api.Models;

namespace MyHealth.Api.Services
{
    public interface IHealthService
    {
        Task<HealthResponse> AssessAsync(HealthRequest request);
        Task<HealthResponse> AssessSimpleAsync(SimpleHealthRequest request);
        Task<AdvancedHealthAnalysisResult> AssessAdvancedHealthAsync(HealthRequest request);
    }
}
