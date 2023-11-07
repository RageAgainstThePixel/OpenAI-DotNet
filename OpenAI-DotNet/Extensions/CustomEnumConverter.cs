using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    internal sealed class CustomEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        private readonly JsonNamingPolicy namingPolicy;
        private readonly Dictionary<string, T> readCache = new();
        private readonly Dictionary<T, JsonEncodedText> writeCache = new();

        // This converter will only support up to 64 enum values (including flags) on serialization and deserialization
        private const int NameCacheLimit = 64;

        private const string ValueSeparator = ", ";

        public CustomEnumConverter(JsonNamingPolicy namingPolicy, JsonSerializerOptions options, object[] knownValues)
        {
            this.namingPolicy = namingPolicy;

            var continueProcessing = true;
            for (var i = 0; i < knownValues?.Length; i++)
            {
                if (!TryProcessValue((T)knownValues[i]))
                {
                    continueProcessing = false;
                    break;
                }
            }

            if (continueProcessing)
            {
                var values = Enum.GetValues(typeof(T));

                for (var i = 0; i < values.Length; i++)
                {
                    var value = (T)values.GetValue(i)!;

                    if (!TryProcessValue(value))
                    {
                        break;
                    }
                }
            }

            bool TryProcessValue(T value)
            {
                if (readCache.Count == NameCacheLimit)
                {
                    Debug.Assert(writeCache.Count == NameCacheLimit);
                    return false;
                }

                FormatAndAddToCaches(value, options.Encoder);
                return true;
            }
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string json;

            if (reader.TokenType != JsonTokenType.String ||
                (json = reader.GetString()) == null ||
                !readCache.TryGetValue(json, out var value))
            {
                throw new JsonException();
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (!writeCache.TryGetValue(value, out var formatted))
            {
                if (writeCache.Count == NameCacheLimit)
                {
                    Debug.Assert(readCache.Count == NameCacheLimit);
                    throw new ArgumentOutOfRangeException(nameof(writeCache));
                }

                formatted = FormatAndAddToCaches(value, options.Encoder);
            }

            writer.WriteStringValue(formatted);
        }

        private JsonEncodedText FormatAndAddToCaches(T value, JavaScriptEncoder encoder)
        {
            var (valueFormattedToStr, valueEncoded) = FormatEnumValue(value.ToString(), namingPolicy, encoder);
            readCache[valueFormattedToStr] = value;
            writeCache[value] = valueEncoded;
            return valueEncoded;
        }

        private static ValueTuple<string, JsonEncodedText> FormatEnumValue(string value, JsonNamingPolicy namingPolicy, JavaScriptEncoder encoder)
        {
            string converted;

            if (!value.Contains(ValueSeparator))
            {
                converted = namingPolicy.ConvertName(value);
            }
            else
            {
                // todo: optimize implementation here by leveraging https://github.com/dotnet/runtime/issues/934.
                var enumValues = value.Split(ValueSeparator);

                for (var i = 0; i < enumValues.Length; i++)
                {
                    enumValues[i] = namingPolicy.ConvertName(enumValues[i]);
                }

                converted = string.Join(ValueSeparator, enumValues);
            }

            return (converted, JsonEncodedText.Encode(converted, encoder));
        }
    }
}