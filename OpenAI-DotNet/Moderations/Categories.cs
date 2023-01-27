using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class Categories
    {
        [JsonConstructor]
        public Categories(bool hate, bool hateThreatening, bool selfHarm, bool sexual, bool sexualMinors, bool violence, bool violenceGraphic)
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
        public bool Hate { get; }

        [JsonPropertyName("hate/threatening")]
        public bool HateThreatening { get; }

        [JsonPropertyName("self-harm")]
        public bool SelfHarm { get; }

        [JsonPropertyName("sexual")]
        public bool Sexual { get; }

        [JsonPropertyName("sexual/minors")]
        public bool SexualMinors { get; }

        [JsonPropertyName("violence")]
        public bool Violence { get; }

        [JsonPropertyName("violence/graphic")]
        public bool ViolenceGraphic { get; }
    }
}