﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Usage
    {
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
    }
}
