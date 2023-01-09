using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class Scores
    {
        [JsonPropertyName("hate")]
        public double Hate { get; set; }

        [JsonPropertyName("hate/threatening")]
        public double HateThreatening { get; set; }

        [JsonPropertyName("self-harm")]
        public double SelfHarm { get; set; }

        [JsonPropertyName("sexual")]
        public double Sexual { get; set; }

        [JsonPropertyName("sexual/minors")]
        public double SexualMinors { get; set; }

        [JsonPropertyName("violence")]
        public double Violence { get; set; }

        [JsonPropertyName("violence/graphic")]
        public double ViolenceGraphic { get; set; }
    }
}