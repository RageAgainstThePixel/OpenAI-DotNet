// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class PromptTokensDetails
    {
        public PromptTokensDetails() { }

        private PromptTokensDetails(
            int? audioTokens = null,
            int? cachedTokens = null)
        {
            AudioTokens = audioTokens;
            CachedTokens = cachedTokens;
        }

        [JsonInclude]
        [JsonPropertyName("audio_tokens")]
        public int? AudioTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("cached_tokens")]
        public int? CachedTokens { get; private set; }

        public static PromptTokensDetails operator +(PromptTokensDetails a, PromptTokensDetails b)
            => new(
                (a?.AudioTokens ?? 0) + (b?.AudioTokens ?? 0),
                (a?.CachedTokens ?? 0) + (b?.CachedTokens ?? 0));
    }
}
