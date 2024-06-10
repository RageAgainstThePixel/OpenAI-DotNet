// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    internal sealed class ResponseFormatConverter : JsonConverter<ChatResponseFormat>
    {
        private class ResponseFormatObject
        {
            public ResponseFormatObject(ChatResponseFormat type) => Type = type;

            [JsonInclude]
            [JsonPropertyName("type")]
            [JsonConverter(typeof(JsonStringEnumConverter<ChatResponseFormat>))]
            public ChatResponseFormat Type { get; private set; }

            public static implicit operator ResponseFormatObject(ChatResponseFormat type) => new(type);

            public static implicit operator ChatResponseFormat(ResponseFormatObject format) => format.Type;
        }

        public override ChatResponseFormat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType is JsonTokenType.Null or JsonTokenType.String)
                {
                    return ChatResponseFormat.Auto;
                }

                return JsonSerializer.Deserialize<ResponseFormatObject>(ref reader, options);
            }
            catch (Exception e)
            {
                throw new Exception($"Error reading {typeof(ChatResponseFormat)} from JSON.", e);
            }
        }

        public override void Write(Utf8JsonWriter writer, ChatResponseFormat value, JsonSerializerOptions options)
        {
            const string type = nameof(type);
            const string text = nameof(text);
            // ReSharper disable once InconsistentNaming
            const string json_object = nameof(json_object);

            switch (value)
            {
                case ChatResponseFormat.Auto:
                    writer.WriteNullValue();
                    break;
                case ChatResponseFormat.Text:
                    writer.WriteStartObject();
                    writer.WriteString(type, text);
                    writer.WriteEndObject();
                    break;
                case ChatResponseFormat.Json:
                    writer.WriteStartObject();
                    writer.WriteString(type, json_object);
                    writer.WriteEndObject();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
