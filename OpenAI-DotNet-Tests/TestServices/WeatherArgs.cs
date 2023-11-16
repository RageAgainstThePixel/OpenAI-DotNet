using System.Text.Json.Serialization;

namespace OpenAI.Tests.Weather
{
    internal class WeatherArgs
    {
        [JsonPropertyName("location")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Location { get; set; }

        [JsonPropertyName("unit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Unit { get; set; }
    }
}