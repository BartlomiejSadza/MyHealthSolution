using System.Text.Json.Serialization;

namespace MyHealth.Api.Models
{
    // Stary format dla Databricks
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

    // Nowy prosty format dla lokalnego ML modelu
    public class SimpleHealthRequest
    {
        public int Age { get; set; }
        public string Gender { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public int VegetableConsumption { get; set; }
        public int NumberOfMeals { get; set; }
        public int WaterConsumption { get; set; }
        public int PhysicalActivityFrequency { get; set; }
        public int TechnologyTime { get; set; }
        public bool FamilyHistoryOverweight { get; set; }
        public bool HighCalorieFood { get; set; }
        public bool Smoking { get; set; }
        public bool CalorieMonitoring { get; set; }
        public int Transportation { get; set; }
    }
}
