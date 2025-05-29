using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MyHealth.Api.Models;

namespace MyHealth.Api.Clients
{
    public class DatabricksModelClient : IModelClient
    {
        private readonly HttpClient _http;
        private readonly string _token;

        public DatabricksModelClient(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _token = cfg["Databricks:Token"];
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);
        }

        public async Task<string[]> PredictAsync(HealthRequest req)
        {
            var opts = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            // serializacja requestu
            var json = JsonSerializer.Serialize(req, opts);
            using var content = new StringContent(
                json, Encoding.UTF8, "application/json");

            // POST na Databricks Serving Endpoint
            var resp = await _http.PostAsync(string.Empty, content);
            resp.EnsureSuccessStatusCode();

            // parsowanie JSONu z predictions
            using var doc = JsonDocument.Parse(
                await resp.Content.ReadAsStringAsync());
            var arr = doc.RootElement.GetProperty("predictions");
            var result = new string[arr.GetArrayLength()];
            int i = 0;
            foreach (var el in arr.EnumerateArray())
            {
                result[i++] = el.GetString();
            }

            return result;
        }
    }
}
