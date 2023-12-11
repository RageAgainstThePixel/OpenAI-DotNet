using System;
using System.Text.Json.Serialization;

namespace OpenAI.Completions
{
    /// <summary>
    /// Represents a completion choice returned by the <see cref="CompletionsEndpoint"/>.
    /// </summary>
    [Obsolete("Deprecated")]
    public sealed class Choice
    {
        /// <summary>
        /// The main text of the completion
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        public string Text { get; private set; }

        /// <summary>
        /// If multiple completion choices we returned, this is the index withing the various choices
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("index")]
        public int Index { get; private set; }

        /// <summary>
        /// If the request specified <see cref="CompletionRequest.LogProbabilities"/>, this contains the list of the most likely tokens.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("logprobs")]
        public LogProbabilities LogProbabilities { get; private set; }

        /// <summary>
        /// If this is the last segment of the completion result, this specifies why the completion has ended.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; private set; }

        /// <summary>
        /// Gets the main text of this completion
        /// </summary>
        public override string ToString() => Text ?? string.Empty;

        public static implicit operator string(Choice choice) => choice.Text;
    }
}
