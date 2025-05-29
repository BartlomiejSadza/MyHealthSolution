using System.Threading.Tasks;
using MyHealth.Api.Models;

namespace MyHealth.Api.Clients
{
    public interface IModelClient
    {
        Task<string[]> PredictAsync(HealthRequest request);
    }
}
