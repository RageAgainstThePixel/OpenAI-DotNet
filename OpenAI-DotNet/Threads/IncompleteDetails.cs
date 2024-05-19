using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.Threads
{
    public sealed class IncompleteDetails
    {
        [JsonInclude]
        [JsonPropertyName("reason")]
        [JsonConverter(typeof(JsonStringEnumConverter<IncompleteMessageReason>))]
        public IncompleteMessageReason Reason { get; private set; }
    }
}
