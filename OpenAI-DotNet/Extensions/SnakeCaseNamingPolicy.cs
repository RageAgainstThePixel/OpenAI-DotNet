using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => StringExtensions.ToSnakeCase(name);
    }

    public class JsonStringSnakeEnumConverter : JsonStringEnumConverter
    {
        public JsonStringSnakeEnumConverter() : base(new SnakeCaseNamingPolicy()) { }
    }
}