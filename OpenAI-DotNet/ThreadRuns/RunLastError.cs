using System.Text.Json.Serialization;

namespace OpenAI.ThreadRuns;

public sealed class RunLastError
{
    /// <summary>
    /// One of server_error or rate_limit_exceeded.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; }
    
    /// <summary>
    /// A human-readable description of the error.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}