// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ResponseFormatObject
    {
        public ResponseFormatObject(ResponseFormat type) => Type = type;

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<ResponseFormat>))]
        public ResponseFormat Type { get; private set; }

        public static implicit operator ResponseFormatObject(ResponseFormat type) => new(type);

        public static implicit operator ResponseFormat(ResponseFormatObject type) => type.Type;
    }
}
