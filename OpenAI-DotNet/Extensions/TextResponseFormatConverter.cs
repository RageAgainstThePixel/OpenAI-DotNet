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
            TextResponseFormatConfiguration result;
            try
            {
                if (reader.TokenType is JsonTokenType.Null or JsonTokenType.String)
                {
                    return TextResponseFormat.Auto;
                }

                var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;

                if (jsonObject.TryGetProperty("type", out var typeProperty) &&
                    typeProperty.ValueKind == JsonValueKind.String)
                {
                    var type = typeProperty.Deserialize<TextResponseFormat>(options);

                    if (type == TextResponseFormat.JsonSchema)
                    {
                        if (!jsonObject.TryGetProperty("json_schema", out var schemaProperty) || schemaProperty.ValueKind != JsonValueKind.Object)
                        {
                            throw new ArgumentException("JsonSchema must be provided when using JsonSchema response format.");
                        }

                        var jsonSchema = schemaProperty.Deserialize<JsonSchema>(options);
                        result = new TextResponseFormatConfiguration(jsonSchema);
                    }
                    else
                    {
                        result = new TextResponseFormatConfiguration(type);
                    }
                }
                else
                {
                    throw new ArgumentException($"Invalid JSON format for TextResponseFormatConfiguration.\n{jsonObject.GetRawText()}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error reading {typeof(TextResponseFormat)} from JSON.", e);
            }

            return result;
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

    internal sealed class ResponseTextResponseFormatConverter : JsonConverter<TextResponseFormatConfiguration>
    {
        public override TextResponseFormatConfiguration Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            TextResponseFormatConfiguration result;
            try
            {
                if (reader.TokenType is JsonTokenType.Null or JsonTokenType.String)
                {
                    return TextResponseFormat.Auto;
                }

                var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;

                if (jsonObject.TryGetProperty("type", out var typeProperty) && typeProperty.ValueKind == JsonValueKind.String)
                {
                    var type = typeProperty.Deserialize<TextResponseFormat>(options);

                    if (type == TextResponseFormat.JsonSchema)
                    {
                        if (!jsonObject.TryGetProperty("schema", out var schemaProperty) || schemaProperty.ValueKind != JsonValueKind.Object)
                        {
                            throw new ArgumentException("JsonSchema must be provided when using JsonSchema response format.");
                        }

                        var jsonSchema = schemaProperty.Deserialize<JsonSchema>(options);
                        result = new TextResponseFormatConfiguration(jsonSchema);
                    }
                    else
                    {
                        result = new TextResponseFormatConfiguration(type);
                    }
                }
                else
                {
                    throw new ArgumentException($"Invalid JSON format for TextResponseFormatConfiguration.\n{jsonObject.GetRawText()}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error reading {typeof(TextResponseFormat)} from JSON.", e);
            }

            return result;
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
                        name = value.JsonSchema.Name,
                        strict = value.JsonSchema.Strict,
                        schema = value.JsonSchema.Schema
                    }, options);
                    break;
                case TextResponseFormat.Text:
                    JsonSerializer.Serialize(writer, new { type = value }, options);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value.Type), $"Unsupported response format: {value.Type}");
            }
        }
    }
}
