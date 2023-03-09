using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class Scores
    {
        [JsonInclude]
        [JsonPropertyName("hate")]
        public double Hate { get; private set; }

        [JsonInclude]
        [JsonPropertyName("hate/threatening")]
        public double HateThreatening { get; private set; }

        [JsonInclude]
        [JsonPropertyName("self-harm")]
        public double SelfHarm { get; private set; }

        [JsonInclude]
        [JsonPropertyName("sexual")]
        public double Sexual { get; private set; }

        [JsonInclude]
        [JsonPropertyName("sexual/minors")]
        public double SexualMinors { get; private set; }

        [JsonInclude]
        [JsonPropertyName("violence")]
        public double Violence { get; private set; }

        [JsonInclude]
        [JsonPropertyName("violence/graphic")]
        public double ViolenceGraphic { get; private set; }
    }
}
