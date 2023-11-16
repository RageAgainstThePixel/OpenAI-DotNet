using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class RunLastError
    {
        /// <summary>
        /// One of server_error or rate_limit_exceeded.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code")]
        public string Code { get; private set; }
    
        /// <summary>
        /// A human-readable description of the error.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message { get; private set; }
    }
}