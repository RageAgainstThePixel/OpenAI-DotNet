// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.Chat
{
    public sealed class ResponseFormat
    {
        public ResponseFormat() => Type = ChatResponseFormat.Text;

        public ResponseFormat(ChatResponseFormat format) => Type = format;

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<ChatResponseFormat>))]
        public ChatResponseFormat Type { get; private set; }

        public static implicit operator ChatResponseFormat(ResponseFormat format) => format.Type;

        public static implicit operator ResponseFormat(ChatResponseFormat format) => new(format);
    }
}