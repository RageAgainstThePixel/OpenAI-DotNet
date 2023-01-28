using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class ModerationsResponse : BaseResponse
    {
        [JsonConstructor]
        public ModerationsResponse(string id, string model, List<ModerationResult> results)
        {
            Id = id;
            Model = model;
            Results = results;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("model")]
        public string Model { get; }

        [JsonPropertyName("results")]
        public List<ModerationResult> Results { get; }
    }
}
