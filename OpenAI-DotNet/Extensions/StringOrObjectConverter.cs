// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    internal sealed class StringOrObjectConverter<T> : JsonConverter<dynamic>
    {
        public override dynamic Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        return null;
                    case JsonTokenType.String:
                        return reader.GetString();
                    case JsonTokenType.StartObject:
                        return JsonSerializer.Deserialize<T>(ref reader, options);
                    default:
                        throw new JsonException($"Unexpected token type: {reader.TokenType}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error reading {typeof(T).Name} from JSON.", e);
            }
        }

        public override void Write(Utf8JsonWriter writer, dynamic value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    break;
                case string stringValue:
                    writer.WriteStringValue(stringValue);
                    break;
                case TextContent textContent:
                    JsonSerializer.Serialize(writer, textContent, options);
                    break;
                default:
                    throw new JsonException($"Unexpected value type: {value.GetType()}");
            }
        }
    }
}
