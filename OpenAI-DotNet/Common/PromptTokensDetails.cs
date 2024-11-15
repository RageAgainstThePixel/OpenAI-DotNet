// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class PromptTokensDetails
    {
        public PromptTokensDetails() { }

        private PromptTokensDetails(
            int? cachedTokens = null,
            int? audioTokens = null,
            int? textTokens = null,
            int? imageTokens = null)
        {
            CachedTokens = cachedTokens;
            AudioTokens = audioTokens;
            TextTokens = textTokens;
            ImageTokens = imageTokens;
        }

        [JsonInclude]
        [JsonPropertyName("cached_tokens")]
        public int? CachedTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("audio_tokens")]
        public int? AudioTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("text_tokens")]
        public int? TextTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("image_tokens")]
        public int? ImageTokens { get; private set; }

        public static PromptTokensDetails operator +(PromptTokensDetails a, PromptTokensDetails b)
            => new(
                (a?.CachedTokens ?? 0) + (b?.CachedTokens ?? 0),
                (a?.AudioTokens ?? 0) + (b?.AudioTokens ?? 0),
                (a?.TextTokens ?? 0) + (b?.TextTokens ?? 0),
                (a?.ImageTokens ?? 0) + (b?.ImageTokens ?? 0));
    }
}
