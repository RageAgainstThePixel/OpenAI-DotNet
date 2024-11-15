// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class RateLimit
    {
        [JsonInclude]
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; private set; }

        [JsonInclude]
        [JsonPropertyName("limit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Limit { get; private set; }

        [JsonInclude]
        [JsonPropertyName("remaining")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Remaining { get; private set; }

        [JsonInclude]
        [JsonPropertyName("reset_seconds")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public float? ResetSeconds { get; private set; }
    }
}
