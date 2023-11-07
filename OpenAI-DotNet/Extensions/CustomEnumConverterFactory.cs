using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    internal sealed class CustomEnumConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            object[] knownValues = null;

            if (typeToConvert == typeof(BindingFlags))
            {
                knownValues = new object[] { BindingFlags.CreateInstance | BindingFlags.DeclaredOnly };
            }

            return (JsonConverter)Activator.CreateInstance(
                typeof(CustomEnumConverter<>).MakeGenericType(typeToConvert),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { new SnakeCaseNamingPolicy(), options, knownValues },
                culture: null)!;
        }
    }
}