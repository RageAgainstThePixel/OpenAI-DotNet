// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    internal sealed class ResponseFormatConverter : JsonConverter<ResponseFormat>
    {
        public override ResponseFormat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType is JsonTokenType.Null or JsonTokenType.String)
                {
                    return ResponseFormat.Auto;
                }

                return JsonSerializer.Deserialize<ResponseFormatObject>(ref reader, options);
            }
            catch (Exception e)
            {
                throw new Exception($"Error reading {typeof(ResponseFormat)} from JSON.", e);
            }
        }

        public override void Write(Utf8JsonWriter writer, ResponseFormat value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case ResponseFormat.Auto:
                    writer.WriteStringValue(value.ToString().ToLower());
                    break;
                case ResponseFormat.Text:
                    writer.WriteStartObject();
                    writer.WriteString("type", "text");
                    writer.WriteEndObject();
                    break;
                case ResponseFormat.Json:
                    writer.WriteStartObject();
                    writer.WriteString("type", "json_object");
                    writer.WriteEndObject();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
