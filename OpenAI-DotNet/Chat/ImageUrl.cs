using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class ImageUrl
    {
        [JsonConstructor]
        public ImageUrl(string url)
        {
            Url = url;
        }

        [JsonInclude]
        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Url { get; private set; }
    }
}