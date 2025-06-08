// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    public sealed class ImageResult
    {
        [JsonInclude]
        [JsonPropertyName("url")]
        public string Url { get; private set; }

        [JsonInclude]
        [JsonPropertyName("b64_json")]
        public string B64_Json { get; private set; }

        [JsonInclude]
        [JsonPropertyName("revised_prompt")]
        public string RevisedPrompt { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; internal set; }

        [JsonIgnore]
        public string Background { get; internal set; }

        [JsonIgnore]
        public string OutputFormat { get; internal set; }

        [JsonIgnore]
        public string Quality { get; internal set; }

        [JsonIgnore]
        public string Size { get; internal set; }

        [JsonIgnore]
        public TokenUsage Usage { get; internal set; }

        public static implicit operator string(ImageResult result) => result?.ToString();

        public override string ToString()
            => !string.IsNullOrWhiteSpace(Url)
                ? Url
                : !string.IsNullOrWhiteSpace(B64_Json)
                    ? B64_Json
                    : string.Empty;
    }
}
