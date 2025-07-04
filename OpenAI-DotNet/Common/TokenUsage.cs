﻿// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class TokenUsage
    {
        /// <summary>
        /// The total number of tokens in the Response including input and output text and audio tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("total_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("input_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? InputTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("input_token_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TokenUsageDetails InputTokenDetails { get; private set; }

        [JsonInclude]
        [JsonPropertyName("output_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? OutputTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("output_token_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TokenUsageDetails OutputTokenDetails { get; private set; }

        public override string ToString()
            => JsonSerializer.Serialize(this, ResponseExtensions.DebugJsonOptions);
    }
}
