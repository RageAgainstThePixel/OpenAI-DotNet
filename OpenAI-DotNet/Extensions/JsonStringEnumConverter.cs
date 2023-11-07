using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    /// <summary>
    /// https://github.com/dotnet/runtime/issues/74385#issuecomment-1456725149
    /// </summary>
    internal sealed class JsonStringEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        private readonly JsonNamingPolicy namingPolicy;
        private readonly Dictionary<int, TEnum> numberToEnum = new();
        private readonly Dictionary<TEnum, string> enumToString = new();
        private readonly Dictionary<string, TEnum> stringToEnum = new();

        public JsonStringEnumConverter()
        {
            // We assume everything from OpenAI is snake case
            namingPolicy = new SnakeCaseNamingPolicy();
            var type = typeof(TEnum);

            foreach (var value in Enum.GetValues<TEnum>())
            {
                var enumMember = type.GetMember(value.ToString())[0];
                var attribute = enumMember.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                    .Cast<EnumMemberAttribute>()
                    .FirstOrDefault();
                var index = Convert.ToInt32(type.GetField("value__")?.GetValue(value));

                if (attribute?.Value != null)
                {
                    numberToEnum.Add(index, value);
                    enumToString.Add(value, attribute.Value);
                    stringToEnum.Add(attribute.Value, value);
                }
                else
                {
                    var convertedName = namingPolicy != null
                        ? namingPolicy.ConvertName(value.ToString())
                        : value.ToString();
                    numberToEnum.Add(index, value);
                    enumToString.Add(value, convertedName);
                    stringToEnum.Add(convertedName, value);
                }
            }
        }

        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var type = reader.TokenType;

            switch (type)
            {
                case JsonTokenType.String:
                {
                    var stringValue = reader.GetString();

                    if (stringValue != null)
                    {
                        var value = namingPolicy != null
                            ? namingPolicy.ConvertName(stringValue)
                            : stringValue;

                        if (stringToEnum.TryGetValue(value, out var enumValue))
                        {
                            return enumValue;
                        }
                    }

                    break;
                }
                case JsonTokenType.Number:
                {
                    var numValue = reader.GetInt32();
                    numberToEnum.TryGetValue(numValue, out var enumValue);
                    return enumValue;
                }
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
            => writer.WriteStringValue(enumToString[value]);
    }
}