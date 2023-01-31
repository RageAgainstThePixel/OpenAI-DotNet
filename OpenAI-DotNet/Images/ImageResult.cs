using System.Text.Json.Serialization;

namespace OpenAI.Images
{
    internal class ImageResult
    {
        [JsonConstructor]
        public ImageResult(string url)
        {
            Url = url;
        }

        [JsonPropertyName("url")]
        public string Url { get; }
    }
}
