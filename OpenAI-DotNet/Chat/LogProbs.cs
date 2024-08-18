// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    /// <summary>
    /// Log probability information for the choice.
    /// </summary>
    public sealed class LogProbs
    {
        /// <summary>
        /// A list of message content tokens with log probability information.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content")]
        public IReadOnlyList<LogProbInfo> Content { get; private set; }

        /// <summary>
        /// A list of message refusal tokens with log probability information.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("refusal")]
        public IReadOnlyList<LogProbInfo> Refusal { get; private set; }
    }
}
