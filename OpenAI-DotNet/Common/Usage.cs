// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Usage
    {
        public Usage() { }

        private Usage(
            int? completionTokens,
            int? promptTokens,
            int? totalTokens,
            CompletionTokensDetails completionTokensDetails,
            PromptTokensDetails promptTokensDetails)
        {
            PromptTokens = promptTokens;
            CompletionTokens = completionTokens;
            TotalTokens = totalTokens;
            CompletionTokensDetails = completionTokensDetails;
            PromptTokensDetails = promptTokensDetails;
        }

        [JsonInclude]
        [JsonPropertyName("prompt_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? PromptTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("completion_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? CompletionTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("total_tokens")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? TotalTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("completion_tokens_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public CompletionTokensDetails CompletionTokensDetails { get; private set; }

        [JsonInclude]
        [JsonPropertyName("prompt_tokens_details")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public PromptTokensDetails PromptTokensDetails { get; private set; }

        internal void AppendFrom(Usage other)
        {
            if (other?.PromptTokens != null)
            {
                PromptTokens = other.PromptTokens.Value;
            }

            if (other?.CompletionTokens != null)
            {
                CompletionTokens = other.CompletionTokens.Value;
            }

            if (other?.TotalTokens != null)
            {
                TotalTokens = other.TotalTokens.Value;
            }

            if (other?.CompletionTokensDetails != null)
            {
                CompletionTokensDetails = other.CompletionTokensDetails;
            }

            if (other?.PromptTokensDetails != null)
            {
                PromptTokensDetails = other.PromptTokensDetails;
            }
        }

        public override string ToString()
            => JsonSerializer.Serialize(this, OpenAIClient.JsonSerializationOptions);

        public static Usage operator +(Usage a, Usage b)
            => new(
                (a.PromptTokens ?? 0) + (b.PromptTokens ?? 0),
                (a.CompletionTokens ?? 0) + (b.CompletionTokens ?? 0),
                (a.TotalTokens ?? 0) + (b.TotalTokens ?? 0),
                a.CompletionTokensDetails + b.CompletionTokensDetails,
                a.PromptTokensDetails + b.PromptTokensDetails);
    }
}
