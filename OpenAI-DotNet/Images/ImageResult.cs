using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    internal class ImageResult
    {
        [JsonInclude]
        [JsonPropertyName("url")]
        public string Url { get; private set; }

        [JsonInclude]
        [JsonPropertyName("b64_json")]
        public string B64_Json { get; private set; }
    }
}
