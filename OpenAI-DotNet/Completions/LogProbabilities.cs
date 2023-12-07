using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Completions
{
    /// <summary>
    /// Object belonging to a <see cref="Choice"/>
    /// </summary>
    [Obsolete("Deprecated")]
    public sealed class LogProbabilities
    {
        [JsonInclude]
        [JsonPropertyName("tokens")]
        public IReadOnlyList<string> Tokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("token_logprobs")]
        public IReadOnlyList<double> TokenLogProbabilities { get; private set; }

        [JsonInclude]
        [JsonPropertyName("top_logprobs")]
        public IReadOnlyList<IReadOnlyDictionary<string, double>> TopLogProbabilities { get; private set; }

        [JsonInclude]
        [JsonPropertyName("text_offset")]
        public IReadOnlyList<int> TextOffsets { get; private set; }
    }
}
