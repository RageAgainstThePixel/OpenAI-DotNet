using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Usage
    {
        public Usage() { }

        public Usage(int promptTokens, int completionTokens, int totalTokens)
        {
            PromptTokens = promptTokens;
            CompletionTokens = completionTokens;
            TotalTokens = totalTokens;
        }

        [JsonInclude]
        [JsonPropertyName("prompt_tokens")]
        public int? PromptTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("completion_tokens")]
        public int? CompletionTokens { get; private set; }

        [JsonInclude]
        [JsonPropertyName("total_tokens")]
        public int? TotalTokens { get; private set; }

        internal void CopyFrom(Usage other)
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
        }

        public override string ToString() => JsonSerializer.Serialize(this);

        public string ToPrintableString() =>
            $"{"PromptTokens:",-18}{PromptTokens}{System.Environment.NewLine}" +
            $"{"CompletionTokens:",-18}{CompletionTokens}{System.Environment.NewLine}" +
            $"{"TotalTokens:",-18}{TotalTokens}{System.Environment.NewLine}";

        public static Usage operator+(Usage a, Usage b)
        {
            return new Usage(
                (a.PromptTokens ?? 0) + (b.PromptTokens ?? 0),
                (a.CompletionTokens ?? 0) + (b.CompletionTokens ?? 0),
                (a.TotalTokens ?? 0) + (b.TotalTokens ?? 0));
        }
    }
}
