using OpenAI.Extensions;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class MessageContent
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<ContentType>))]
        public ContentType Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public TextContent Text { get; private set; }

        [JsonInclude]
        [JsonPropertyName("image_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImageUrl ImageUrl { get; private set; }
    }
}