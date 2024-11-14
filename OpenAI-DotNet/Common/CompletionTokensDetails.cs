// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class CompletionTokensDetails
    {
        public CompletionTokensDetails() { }

        private CompletionTokensDetails(
            int? acceptedPredictionTokens,
            int? audioTokens,
            int? reasoningTokens,
            int? rejectedPredictionTokens)
        {
            AcceptedPredictionTokens = acceptedPredictionTokens;
            AudioTokens = audioTokens;
            ReasoningTokens = reasoningTokens;
            RejectedPredictionTokens = rejectedPredictionTokens;
        }

        [JsonInclude]
        [JsonPropertyName("accepted_prediction_tokens")]
        public int? AcceptedPredictionTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("audio_tokens")]
        public int? AudioTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("reasoning_tokens")]
        public int? ReasoningTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("rejected_prediction_tokens")]
        public int? RejectedPredictionTokens { get; private set; }

        public static CompletionTokensDetails operator +(CompletionTokensDetails a, CompletionTokensDetails b)
            => new(
                (a?.AcceptedPredictionTokens ?? 0) + (b?.AcceptedPredictionTokens ?? 0),
                (a?.AudioTokens ?? 0) + (b?.AudioTokens ?? 0),
                (a?.ReasoningTokens ?? 0) + (b?.ReasoningTokens ?? 0),
                (a?.RejectedPredictionTokens ?? 0) + (b?.RejectedPredictionTokens ?? 0));
    }
}
