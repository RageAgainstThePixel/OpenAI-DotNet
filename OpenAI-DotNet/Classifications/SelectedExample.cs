using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class SelectedExample
    {
        [JsonPropertyName("document")]
        public int Document { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}