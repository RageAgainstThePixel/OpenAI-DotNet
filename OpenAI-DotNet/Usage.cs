using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Usage
    {
        [JsonConstructor]
        public Usage(
            int promptTokens,
            int completionTokens,
            int totalTokens)
        {
            PromptTokens = promptTokens;
            CompletionTokens = completionTokens;
            TotalTokens = totalTokens;
        }

        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; }
    }
}
