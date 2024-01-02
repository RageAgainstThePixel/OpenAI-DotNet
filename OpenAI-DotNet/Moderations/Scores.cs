// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
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
        [JsonPropertyName("harassment")]
        public double Harassment { get; private set; }

        [JsonInclude]
        [JsonPropertyName("harassment/threatening")]
        public double HarassmentThreatening { get; private set; }

        [JsonInclude]
        [JsonPropertyName("self-harm")]
        public double SelfHarm { get; private set; }

        [JsonInclude]
        [JsonPropertyName("self-harm/intent")]
        public double SelfHarmIntent { get; private set; }

        [JsonInclude]
        [JsonPropertyName("self-harm/instructions")]
        public double SelfHarmInstructions { get; private set; }

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

        public override string ToString() =>
            $"Hate :{Hate:0.00 E+00}{Environment.NewLine}" +
            $"Hate/Threatening: {HateThreatening:0.00 E+00}{Environment.NewLine}" +
            $"Harassment :{Harassment:0.00 E+00}{Environment.NewLine}" +
            $"Harassment/Threatening: {HarassmentThreatening:0.00 E+00}{Environment.NewLine}" +
            $"Self-Harm: {SelfHarm:0.00 E+00}{Environment.NewLine}" +
            $"Self-Harm/Intent: {SelfHarmIntent:0.00 E+00}{Environment.NewLine}" +
            $"Self-Harm/Instructions: {SelfHarmInstructions:0.00 E+00}{Environment.NewLine}" +
            $"Sexual: {Sexual:0.00 E+00}{Environment.NewLine}" +
            $"Sexual/Minors: {SexualMinors:0.00 E+00}{Environment.NewLine}" +
            $"Violence: {Violence:0.00 E+00}{Environment.NewLine}" +
            $"Violence/Graphic: {ViolenceGraphic:0.00 E+00}{Environment.NewLine}";

        public static implicit operator string(Scores scores) => scores.ToString();
    }
}
