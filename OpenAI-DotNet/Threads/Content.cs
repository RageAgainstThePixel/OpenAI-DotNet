using OpenAI.Extensions;
using System;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class Content
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

        public override string ToString()
            => Type switch
            {
                ContentType.Text => Text.Value,
                ContentType.ImageUrl => ImageUrl.Url,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}