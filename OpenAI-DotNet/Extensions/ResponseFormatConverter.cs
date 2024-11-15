// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    internal sealed class ResponseFormatConverter : JsonConverter<ResponseFormatObject>
    {
        public override ResponseFormatObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        public override void Write(Utf8JsonWriter writer, ResponseFormatObject value, JsonSerializerOptions options)
        {
            switch (value.Type)
            {
                case ChatResponseFormat.Auto:
                    // ignore
                    break;
                default:
                    JsonSerializer.Serialize(writer, value, options);
                    break;
            }
        }
    }
}
