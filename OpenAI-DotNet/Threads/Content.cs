// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        [JsonPropertyName("image_file")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ImageFile ImageFile { get; private set; }

        public override string ToString()
            => Type switch
            {
                ContentType.Text => Text.Value,
                ContentType.ImageFile => ImageFile.FileId,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}