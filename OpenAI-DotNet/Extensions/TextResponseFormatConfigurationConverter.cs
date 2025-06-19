// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    internal sealed class TextResponseFormatConfigurationConverter : JsonConverter<TextResponseFormatConfiguration>
    {
        public override TextResponseFormatConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        return TextResponseFormat.Auto;
                    case JsonTokenType.String:
                        return JsonSerializer.Deserialize<TextResponseFormat>(ref reader, options);
                }

                var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;

                if (jsonObject.TryGetProperty("type", out var typeProperty) &&
                    typeProperty.ValueKind == JsonValueKind.String)
                {
                    var type = typeProperty.Deserialize<TextResponseFormat>(options);

                    if (type == TextResponseFormat.JsonSchema)
                    {
                        if (!jsonObject.TryGetProperty("json_schema", out var schemaProperty) ||
                            schemaProperty.ValueKind != JsonValueKind.Object)
                        {
                            throw new ArgumentException("JsonSchema must be provided when using JsonSchema response format.");
                        }

                        var jsonSchema = schemaProperty.Deserialize<JsonSchema>(options);
                        return new TextResponseFormatConfiguration(jsonSchema);
                    }

                    return type;
                }

                throw new ArgumentException($"Invalid JSON format for TextResponseFormatConfiguration.\n{jsonObject.GetRawText()}");
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
                case TextResponseFormat.JsonSchema:
                    if (value.JsonSchema == null)
                    {
                        throw new ArgumentNullException(nameof(value.JsonSchema), "JsonSchema cannot be null when using Json or JsonSchema response formats.");
                    }
                    JsonSerializer.Serialize(writer, new
                    {
                        type = value.Type,
                        json_schema = value.JsonSchema
                    }, options);
                    break;
#pragma warning disable CS0618 // Type or member is obsolete
                case TextResponseFormat.Json:
#pragma warning restore CS0618 // Type or member is obsolete
                case TextResponseFormat.Text:
                    JsonSerializer.Serialize(writer, new { type = value.Type }, options);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value.Type), $"Unsupported response format: {value.Type}");
            }
        }
    }
}
