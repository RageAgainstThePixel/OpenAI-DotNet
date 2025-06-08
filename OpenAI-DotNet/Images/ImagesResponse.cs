// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    internal sealed class ImagesResponse : BaseResponse
    {
        [JsonInclude]
        [JsonPropertyName("created")]
        public long CreatedAtUnixSeconds { get; private set; }

        [JsonInclude]
        [JsonPropertyName("data")]
        public IReadOnlyList<ImageResult> Results { get; private set; }

        [JsonInclude]
        [JsonPropertyName("background")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Background { get; private set; }

        [JsonInclude]
        [JsonPropertyName("output_format")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string OutputFormat { get; private set; }

        [JsonInclude]
        [JsonPropertyName("quality")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Quality { get; private set; }

        [JsonInclude]
        [JsonPropertyName("size")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Size { get; private set; }

        [JsonInclude]
        [JsonPropertyName("usage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TokenUsage Usage { get; private set; }
    }
}
