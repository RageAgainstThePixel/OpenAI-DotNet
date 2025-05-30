// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    internal sealed class TextResponseFormatConverter : JsonConverter<TextResponseFormatConfiguration>
    {
        public override TextResponseFormatConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType is JsonTokenType.Null or JsonTokenType.String)
                {
                    return TextResponseFormat.Auto;
                }

                return JsonSerializer.Deserialize<TextResponseFormatConfiguration>(ref reader, options);
            }
            catch (Exception e)
            {
                throw new Exception($"Error reading {typeof(TextResponseFormat)} from JSON.", e);
            }
        }

        public override void Write(Utf8JsonWriter writer, TextResponseFormatConfiguration value, JsonSerializerOptions options)
        {
            switch (value.Type)
            {
                case TextResponseFormat.Auto:
                    // ignore
                    break;
                default:
                    JsonSerializer.Serialize(writer, value, options);
                    break;
            }
        }
    }
}
