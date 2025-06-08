// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class TokenUsageDetails
    {
        /// <summary>
        /// The number of cached tokens used in the Response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cached_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CachedTokens { get; private set; }

        /// <summary>
        /// The number of text tokens used in the Response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TextTokens { get; private set; }

        /// <summary>
        /// The number of audio tokens used in the Response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("audio_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AudioTokens { get; private set; }

        /// <summary>
        /// The number of image tokens used in the Response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("image_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ImageTokens { get; private set; }
    }
}
