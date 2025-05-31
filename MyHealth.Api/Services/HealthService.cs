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
        private readonly AdvancedHealthAnalyzer _analyzer;

        public HealthService(IModelClient modelClient, IConfiguration cfg, AdvancedHealthAnalyzer analyzer)
        {
            _modelClient = modelClient;
            _analyzer = analyzer;
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

        public async Task<AdvancedHealthAnalysisResult> AssessAdvancedHealthAsync(HealthRequest request)
        {
            try
            {
                // Najpierw pobierz podstawową predykcję
                var basicResult = await AssessAsync(request);
                
                // Następnie wykonaj zaawansowaną analizę
                var advancedResult = _analyzer.AnalyzeHealth(request, basicResult.Assessments);
                
                return advancedResult;
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Błąd podczas zaawansowanej analizy zdrowotnej: {ex.Message}", ex);
            }
        }

        public async Task<HealthResponse> AssessSimpleAsync(
            SimpleHealthRequest request)
        {
            // Konwertuj SimpleHealthRequest na HealthRequest
            var healthRequest = new HealthRequest
            {
                DataFrame_Split = new DataFrameSplit
                {
                    Columns = new[]
                    {
                        "Age", "Gender", "Height", "Weight", "FCVC", "NCP", "CH2O", 
                        "FAF", "TUE", "family_history_with_overweight", "FAVC", 
                        "CAEC", "SMOKE", "SCC", "CALC", "MTRANS"
                    },
                    Data = new object[][]
                    {
                        new object[]
                        {
                            request.Age,
                            request.Gender,
                            request.Height,
                            request.Weight,
                            request.VegetableConsumption,
                            request.NumberOfMeals,
                            request.WaterConsumption,
                            request.PhysicalActivityFrequency,
                            request.TechnologyTime,
                            request.FamilyHistoryOverweight ? "yes" : "no",
                            request.HighCalorieFood ? "yes" : "no",
                            "Sometimes", // CAEC - domyślna wartość
                            request.Smoking ? "yes" : "no",
                            request.CalorieMonitoring ? "yes" : "no",
                            "Sometimes", // CALC - domyślna wartość
                            MapTransportation(request.Transportation)
                        }
                    }
                }
            };

            // Użyj istniejącej metody
            return await AssessAsync(healthRequest);
        }

        private string MapTransportation(int transportation)
        {
            return transportation switch
            {
                0 => "Walking",
                1 => "Public_Transportation",
                2 => "Automobile",
                3 => "Bike",
                _ => "Public_Transportation"
            };
        }
    }
}
