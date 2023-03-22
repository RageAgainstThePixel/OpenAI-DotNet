using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Usage
    {
        [JsonInclude]
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; private set; }
    }
}
