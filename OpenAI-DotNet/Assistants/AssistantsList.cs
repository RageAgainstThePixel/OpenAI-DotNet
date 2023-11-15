using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Assistants
{
    public sealed class AssistantsList : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("data")]
        public IReadOnlyList<Assistant> Assistants { get; private set; }

        [JsonInclude]
        [JsonPropertyName("first_id")]
        public string FirstId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("last_id")]
        public string LastId { get; private set; }

        [JsonInclude]
        [JsonPropertyName("has_more")]
        public bool HasMore { get; private set; }
    }
}