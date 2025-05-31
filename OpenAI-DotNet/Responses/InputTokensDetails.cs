// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    ///  A detailed breakdown of the input tokens.
    /// </summary>
    public sealed class InputTokensDetails
    {
        /// <summary>
        /// The number of tokens that were retrieved from the cache.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("cached_tokens")]
        public int CachedTokens { get; private set; }
    }
}
