using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class Scores
    {
        [JsonConstructor]
        public Scores(double hate, double hateThreatening, double selfHarm, double sexual, double sexualMinors, double violence, double violenceGraphic)
        {
            Hate = hate;
            HateThreatening = hateThreatening;
            SelfHarm = selfHarm;
            Sexual = sexual;
            SexualMinors = sexualMinors;
            Violence = violence;
            ViolenceGraphic = violenceGraphic;
        }

        [JsonPropertyName("hate")]
        public double Hate { get; }

        [JsonPropertyName("hate/threatening")]
        public double HateThreatening { get; }

        [JsonPropertyName("self-harm")]
        public double SelfHarm { get; }

        [JsonPropertyName("sexual")]
        public double Sexual { get; }

        [JsonPropertyName("sexual/minors")]
        public double SexualMinors { get; }

        [JsonPropertyName("violence")]
        public double Violence { get; }

        [JsonPropertyName("violence/graphic")]
        public double ViolenceGraphic { get; }
    }
}
