// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class CompletionTokensDetails
    {
        public CompletionTokensDetails() { }

        private CompletionTokensDetails(
            int? reasoningTokens,
            int? audioTokens,
            int? textTokens,
            int? acceptedPredictionTokens,
            int? rejectedPredictionTokens)
        {
            ReasoningTokens = reasoningTokens;
            AudioTokens = audioTokens;
            TextTokens = textTokens;
            AcceptedPredictionTokens = acceptedPredictionTokens;
            RejectedPredictionTokens = rejectedPredictionTokens;
        }

        [JsonInclude]
        [JsonPropertyName("reasoning_tokens")]
        public int? ReasoningTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("audio_tokens")]
        public int? AudioTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("text_tokens")]
        public int? TextTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("accepted_prediction_tokens")]
        public int? AcceptedPredictionTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("rejected_prediction_tokens")]
        public int? RejectedPredictionTokens { get; private set; }

        public static CompletionTokensDetails operator +(CompletionTokensDetails a, CompletionTokensDetails b)
            => new(
                (a?.ReasoningTokens ?? 0) + (b?.ReasoningTokens ?? 0),
                (a?.AudioTokens ?? 0) + (b?.AudioTokens ?? 0),
                (a?.TextTokens ?? 0) + (b?.TextTokens ?? 0),
                (a?.AcceptedPredictionTokens ?? 0) + (b?.AcceptedPredictionTokens ?? 0),
                (a?.RejectedPredictionTokens ?? 0) + (b?.RejectedPredictionTokens ?? 0));
    }
}
