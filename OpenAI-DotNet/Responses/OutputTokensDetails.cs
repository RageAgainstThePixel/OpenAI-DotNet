// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// A detailed breakdown of the output tokens.
    /// </summary>
    public sealed class OutputTokensDetails
    {
        /// <summary>
        /// The number of reasoning tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("reasoning_tokens")]
        public int ReasoningTokens { get; }
    }
}
