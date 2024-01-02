// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class ModerationResult
    {
        [JsonInclude]
        [JsonPropertyName("categories")]
        public Categories Categories { get; private set; }

        [JsonInclude]
        [JsonPropertyName("category_scores")]
        public Scores Scores { get; private set; }

        [JsonInclude]
        [JsonPropertyName("flagged")]
        public bool Flagged { get; private set; }
    }
}
