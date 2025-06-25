// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    internal class FilterConverter : JsonConverter<IFilter>
    {
        public override void Write(Utf8JsonWriter writer, IFilter value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), options);

        public override IFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var root = JsonDocument.ParseValue(ref reader).RootElement;
            var type = root.GetProperty("type").GetString()!;
            switch (type)
            {
                case "eq":
                case "ne":
                case "gt":
                case "gte":
                case "lt":
                case "lte":
                    return root.Deserialize<ComparisonFilter>(options);
                case "or":
                case "and":
                    return root.Deserialize<CompoundFilter>(options);
                default:
                    throw new NotImplementedException($"Unknown filter type: {type}");
            }
        }
    }
}
