// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.VectorStores
{
    public sealed class ChunkingStrategy
    {
        public ChunkingStrategy() { }

        public ChunkingStrategy(ChunkingStrategyType type)
        {
            Type = type;

            switch (Type)
            {
                case ChunkingStrategyType.Static:
                    Static = new ChunkingStrategyStatic();
                    break;
            }
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<ChunkingStrategyType>))]
        public ChunkingStrategyType Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("static")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ChunkingStrategyStatic Static { get; private set; }
    }
}
