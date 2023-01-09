using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class Categories
    {
        [JsonPropertyName("hate")]
        public bool Hate { get; set; }

        [JsonPropertyName("hate/threatening")]
        public bool HateThreatening { get; set; }

        [JsonPropertyName("self-harm")]
        public bool SelfHarm { get; set; }

        [JsonPropertyName("sexual")]
        public bool Sexual { get; set; }

        [JsonPropertyName("sexual/minors")]
        public bool SexualMinors { get; set; }

        [JsonPropertyName("violence")]
        public bool Violence { get; set; }

        [JsonPropertyName("violence/graphic")]
        public bool ViolenceGraphic { get; set; }
    }
}