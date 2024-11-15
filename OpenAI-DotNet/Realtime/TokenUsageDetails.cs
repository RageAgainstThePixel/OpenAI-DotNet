// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class TokenUsageDetails
    {
        /// <summary>
        /// The number of cached tokens used in the Response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cached_tokens")]
        public int? CachedTokens { get; private set; }

        /// <summary>
        /// The number of text tokens used in the Response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text_tokens")]
        public int? TextTokens { get; private set; }

        /// <summary>
        /// The number of audio tokens used in the Response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("audio_tokens")]
        public int? AudioTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("image_tokens")]
        public int? ImageTokens { get; private set; }
    }
}
