using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyHealth.Api.Models;

namespace MyHealth.Api.Clients
{
    public class LocalModelClient : IModelClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<LocalModelClient> _logger;

        public LocalModelClient(HttpClient http, ILogger<LocalModelClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<string[]> PredictAsync(HealthRequest req)
        {
            try
            {
                var opts = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                // Serializacja requestu
                var json = JsonSerializer.Serialize(req, opts);
                _logger.LogInformation($"Wysyłam do ML modelu: {json}");
                
                using var content = new StringContent(
                    json, Encoding.UTF8, "application/json");

                // POST na lokalny kontener ML
                var resp = await _http.PostAsync("/predict", content);
                
                if (!resp.IsSuccessStatusCode)
                {
                    var errorContent = await resp.Content.ReadAsStringAsync();
                    _logger.LogError($"Błąd ML modelu: {resp.StatusCode} - {errorContent}");
                    throw new HttpRequestException($"ML model error: {resp.StatusCode}");
                }

                // Parsowanie JSONu z predictions
                var responseContent = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation($"Odpowiedź z ML modelu: {responseContent}");
                
                using var doc = JsonDocument.Parse(responseContent);
                var arr = doc.RootElement.GetProperty("predictions");
                var result = new string[arr.GetArrayLength()];
                int i = 0;
                foreach (var el in arr.EnumerateArray())
                {
                    result[i++] = el.GetString();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas komunikacji z ML modelem");
                throw;
            }
        }
    }
}