// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class TextResponseFormatObject
    {
        public TextResponseFormatObject() { }

        public TextResponseFormatObject(TextResponseFormatConfiguration format)
        {
            Format = format ?? throw new ArgumentNullException(nameof(format), "Format cannot be null.");
        }

        [JsonInclude]
        [JsonPropertyName("format")]
        [JsonConverter(typeof(ResponseTextResponseObjectConverter))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TextResponseFormatConfiguration Format { get; private set; }

        public static implicit operator TextResponseFormatObject(TextResponseFormatConfiguration config) => new(config);

        public static implicit operator TextResponseFormat(TextResponseFormatObject formatObj) => formatObj?.Format?.Type ?? TextResponseFormat.Auto;

        public static implicit operator TextResponseFormatObject(TextResponseFormat format) => new(new(format));

        public static implicit operator TextResponseFormatObject(JsonSchema schema) => new(new(schema));
    }
}
