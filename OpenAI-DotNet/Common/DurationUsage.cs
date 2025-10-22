// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class DurationUsage
    {
        [JsonInclude]
        [JsonPropertyName("seconds")]
        public float Seconds { get; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; }

        public override string ToString()
            => JsonSerializer.Serialize(this, ResponseExtensions.DebugJsonOptions);
    }
}
