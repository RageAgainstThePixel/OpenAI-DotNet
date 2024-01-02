// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Error
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