using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyHealth.Api.Clients;
using MyHealth.Api.Models;

namespace MyHealth.Api.Services
{
    public class HealthService : IHealthService
    {
        private readonly IModelClient _modelClient;
        private readonly IDictionary<string, string> _descriptions;

        public HealthService(IModelClient modelClient, IConfiguration cfg)
        {
            _modelClient = modelClient;
            _descriptions = cfg.GetSection("HealthDescriptions")
                               .Get<Dictionary<string, string>>();
        }

        public async Task<HealthResponse> AssessAsync(
            HealthRequest request)
        {
            var preds = await _modelClient.PredictAsync(request);

            var cols = request.DataFrame_Split.Columns;
            var rows = request.DataFrame_Split.Data;
            var assessments = new List<AssessmentItem>();

            for (int i = 0; i < preds.Length; i++)
            {
                // zmapuj kolumny → wartości wiersza
                var features = new Dictionary<string, object>();
                for (int j = 0; j < cols.Length; j++)
                {
                    features[cols[j]] = rows[i][j];
                }

                // opis z configu
                _descriptions.TryGetValue(preds[i], out var desc);

                // obserwacje wg reguł
                var obs = HealthAnalyzer.GenerateObservations(
                    features, preds[i]);

                assessments.Add(new AssessmentItem
                {
                    Prediction   = preds[i],
                    Description  = desc ?? "Brak opisu",
                    Observations = obs
                });
            }

            return new HealthResponse { Assessments = assessments };
        }
    }
}
