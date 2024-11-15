// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ResponseFormatObject
    {
        public ResponseFormatObject() { }

        public ResponseFormatObject(ChatResponseFormat type)
        {
            if (type == ChatResponseFormat.JsonSchema)
            {
                throw new System.ArgumentException("Use the constructor overload that accepts a JsonSchema object for ChatResponseFormat.JsonSchema.", nameof(type));
            }
            Type = type;
        }

        public ResponseFormatObject(JsonSchema schema)
        {
            Type = ChatResponseFormat.JsonSchema;
            JsonSchema = schema;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<ChatResponseFormat>))]
        public ChatResponseFormat Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("json_schema")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public JsonSchema JsonSchema { get; private set; }

        public static implicit operator ResponseFormatObject(ChatResponseFormat type) => new(type);

        public static implicit operator ChatResponseFormat(ResponseFormatObject format) => format.Type;
    }
}
