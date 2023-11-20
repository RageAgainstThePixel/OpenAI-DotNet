namespace OpenAI.Tests.Weather
{
    internal class WeatherService
    {
        public static string GetCurrentWeather(WeatherArgs weatherArgs)
            => $"The current weather in {weatherArgs.Location} is 20 {weatherArgs.Unit}";
    }
}