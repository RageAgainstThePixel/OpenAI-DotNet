// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Extensions
{
    /// <summary>
    /// https://github.com/dotnet/runtime/issues/74385#issuecomment-1456725149
    /// </summary>
    internal sealed class JsonStringEnumConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert.IsEnum;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            => (JsonConverter)Activator.CreateInstance(typeof(JsonStringEnumConverter<>).MakeGenericType(typeToConvert))!;
    }
}