#nullable enable
using System.Text.Json.Serialization;

namespace OpenAI
{
    public class ApiError(string message, string type, string? code, string? param)
    {
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message { get; } = message;

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; } = type;

        [JsonInclude]
        [JsonPropertyName("code")]
        public string? Code { get; } = code;

        [JsonInclude]
        [JsonPropertyName("param")]
        public string? Param { get; } = param;
    }

    public class ApiErrorResponse
    {
        [JsonInclude]
        [JsonPropertyName("error")]
        public ApiError? Error { get; init; }
    }
}
