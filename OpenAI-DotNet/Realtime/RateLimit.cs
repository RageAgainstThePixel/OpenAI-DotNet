// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class RateLimit
    {
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        [JsonInclude]
        [JsonPropertyName("limit")]
        public int Limit { get; private set; }

        [JsonInclude]
        [JsonPropertyName("remaining")]
        public int Remaining { get; private set; }

        [JsonInclude]
        [JsonPropertyName("reset_seconds")]
        public int ResetSeconds { get; private set; }
    }
}
