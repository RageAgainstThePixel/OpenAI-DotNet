using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Assistants
{
    public sealed class AssistantsList
    {
        [JsonPropertyName("object")]
        public string Object { get; set; } = "list";

        [JsonPropertyName("data")]
        public IReadOnlyList<Assistant> Data { get; set; }

        [JsonPropertyName("first_id")]
        public string FirstId { get; set; }

        [JsonPropertyName("last_id")]
        public string LastId { get; set; }

        [JsonPropertyName("has_more")]
        public bool HasMore { get; set; }
    }
}