using System.Text.Json.Serialization;

namespace OpenAI.Completions
{
    /// <summary>
    /// Represents a completion choice returned by the <see cref="CompletionsEndpoint"/>.
    /// </summary>
    public sealed class Choice
    {
        [JsonConstructor]
        public Choice(string text, int index, Logprobs logprobs, string finishReason)
        {
            Text = text;
            Index = index;
            Logprobs = logprobs;
            FinishReason = finishReason;
        }

        /// <summary>
        /// The main text of the completion
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; }

        /// <summary>
        /// If multiple completion choices we returned, this is the index withing the various choices
        /// </summary>
        [JsonPropertyName("index")]
        public int Index { get; }

        /// <summary>
        /// If the request specified <see cref="CompletionRequest.LogProbabilities"/>, this contains the list of the most likely tokens.
        /// </summary>
        [JsonPropertyName("logprobs")]
        public Logprobs Logprobs { get; }

        /// <summary>
        /// If this is the last segment of the completion result, this specifies why the completion has ended.
        /// </summary>
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; }

        /// <summary>
        /// Gets the main text of this completion
        /// </summary>
        public override string ToString() => Text;

        public static implicit operator string(Choice choice) => choice.Text;
    }
}
