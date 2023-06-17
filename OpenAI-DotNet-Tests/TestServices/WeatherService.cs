using System.Text.Json.Serialization;

namespace OpenAI.Tests.Weather
{
    internal class WeatherService
    {
        public static string GetCurrentWeather(WeatherArgs weatherArgs)
        {
            return $"The current weather in {weatherArgs.Location} is 20 {weatherArgs.Unit}";
        }
    }

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