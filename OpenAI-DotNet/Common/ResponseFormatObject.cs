// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal sealed class ResponseFormatObject
    {
        public ResponseFormatObject(ChatResponseFormat type) => Type = type;

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<ChatResponseFormat>))]
        public ChatResponseFormat Type { get; private set; }

        public static implicit operator ResponseFormatObject(ChatResponseFormat type) => new(type);

        public static implicit operator ChatResponseFormat(ResponseFormatObject format) => format.Type;
    }
}
