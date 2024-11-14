// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class Usage
    {
        /// <summary>
        /// The total number of tokens in the Response including input and output text and audio tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("total_tokens")]
        public int? TotalTokens { get; }

        [JsonInclude]
        [JsonPropertyName("input_tokens")]
        public int? InputTokens { get; }

        [JsonInclude]
        [JsonPropertyName("output_tokens")]
        public int? OutputTokens { get; }

        [JsonInclude]
        [JsonPropertyName("input_token_details")]
        public TokenUsageDetails InputTokenDetails { get; }

        [JsonInclude]
        [JsonPropertyName("output_token_details")]
        public TokenUsageDetails OutputTokenDetails { get; }
    }
}
