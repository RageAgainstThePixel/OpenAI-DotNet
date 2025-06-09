// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    internal class ToolConverter : JsonConverter<ITool>
    {
        public override void Write(Utf8JsonWriter writer, ITool value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), options);

        public override ITool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var root = JsonDocument.ParseValue(ref reader).RootElement;
            var type = root.GetProperty("type").GetString()!;

            return type switch
            {
                "function" => root.Deserialize<Function>(options),
                _ => throw new NotImplementedException($"Unknown tool item type: {type}")
            };
        }
    }
}
