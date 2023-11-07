using System.Text.Json;

namespace OpenAI.Extensions
{
    internal sealed class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
            => StringExtensions.ToSnakeCase(name);
    }
}