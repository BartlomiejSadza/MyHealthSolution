using System.Text.Json.Serialization;

namespace MyHealth.Api.Models
{
    public class HealthRequest
    {
        [JsonPropertyName("dataframe_split")]
        public DataFrameSplit DataFrame_Split { get; set; }
    }

    public class DataFrameSplit
    {
        // poniżej camelCase działa OK → "columns"
        public string[] Columns { get; set; }
        // a to → "data"
        public object[][] Data { get; set; }
    }
}
