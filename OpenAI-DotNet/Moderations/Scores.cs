using System.Text;
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

        public override string ToString()
        {
            var sb = new StringBuilder()
                .AppendLine($"{"Hate:".PadRight(10)}{Hate:0.00 E+00}")
                .AppendLine($"{"Threat:".PadRight(10)}{HateThreatening:0.00 E+00}")
                .AppendLine($"{"Violence:".PadRight(10)}{Violence:0.00 E+00}")
                .AppendLine($"{"Graphic:".PadRight(10)}{ViolenceGraphic:0.00 E+00}")
                .AppendLine($"{"SelfHarm:".PadRight(10)}{SelfHarm:0.00 E+00}")
                .AppendLine($"{"Sexual:".PadRight(10)}{Sexual:0.00 E+00}")
                .AppendLine($"{"Minors:".PadRight(10)}{SexualMinors:0.00 E+00}");
            return sb.ToString();
        }

        public static implicit operator string(Scores scores) => scores.ToString();
    }
}
