using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    internal class ImageResult
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}