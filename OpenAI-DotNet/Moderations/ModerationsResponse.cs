using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class ModerationsResponse : BaseResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("results")]
        public List<ModerationResult> Results { get; set; }
    }
}