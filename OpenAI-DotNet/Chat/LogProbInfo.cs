// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    /// <summary>
    /// Contains log probability information.
    /// </summary>
    public sealed class LogProbInfo
    {
        /// <summary>
        /// The token.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("token")]
        public string Token { get; private set; }

        /// <summary>
        /// The log probability of this token.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logprob")]
        public float LogProb { get; private set; }

        /// <summary>
        /// A list of integers representing the UTF-8 bytes representation of the token.
        /// Useful in instances where characters are represented by multiple tokens and their byte
        /// representations must be combined to generate the correct text representation.
        /// Can be null if there is no bytes representation for the token.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("bytes")]
        public int[] Bytes { get; private set; }

        /// <summary>
        /// List of the most likely tokens and their log probability, at this token position.
        /// In rare cases, there may be fewer than the number of requested top_logprobs returned.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("top_logprobs")]
        public IReadOnlyList<LogProbInfo> TopLogProbs { get; private set; }
    }
}