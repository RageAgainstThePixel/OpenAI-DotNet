// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// Represents token usage details including input tokens, output tokens, a breakdown of output tokens, and the total tokens used.
    /// </summary>
    public sealed class ResponseUsage
    {
        /// <summary>
        /// The number of input tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; private set; }

        /// <summary>
        ///  A detailed breakdown of the input tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("input_tokens_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InputTokensDetails InputTokensDetails { get; private set; }

        /// <summary>
        /// The number of output tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; private set; }

        /// <summary>
        /// A detailed breakdown of the output tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_tokens_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OutputTokensDetails OutputTokensDetails { get; private set; }

        /// <summary>
        /// The total number of tokens used.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; private set; }

        public override string ToString()
            => JsonSerializer.Serialize(this, ResponseExtensions.DebugJsonOptions);
    }
}
