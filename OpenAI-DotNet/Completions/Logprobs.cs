using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Completions
{
    /// <summary>
    /// Object belonging to a <see cref="Choice"/>
    /// </summary>
    public sealed class Logprobs
    {
        [JsonConstructor]
        public Logprobs(List<string> tokens, List<double> tokenLogprobs, IList<IDictionary<string, double>> topLogprobs, List<int> textOffsets)
        {
            Tokens = tokens;
            TokenLogprobs = tokenLogprobs;
            TopLogprobs = topLogprobs;
            TextOffsets = textOffsets;
        }

        [JsonPropertyName("tokens")]
        public List<string> Tokens { get; }

        [JsonPropertyName("token_logprobs")]
        public List<double> TokenLogprobs { get; }

        [JsonPropertyName("top_logprobs")]
        public IList<IDictionary<string, double>> TopLogprobs { get; }

        [JsonPropertyName("text_offset")]
        public List<int> TextOffsets { get; }
    }
}
