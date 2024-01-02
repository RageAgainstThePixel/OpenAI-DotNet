// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Moderations
{
    public sealed class Categories
    {
        [JsonInclude]
        [JsonPropertyName("hate")]
        public bool Hate { get; private set; }

        [JsonInclude]
        [JsonPropertyName("hate/threatening")]
        public bool HateThreatening { get; private set; }

        [JsonInclude]
        [JsonPropertyName("harassment")]
        public bool Harassment { get; private set; }

        [JsonInclude]
        [JsonPropertyName("harassment/threatening")]
        public bool HarassmentThreatening { get; private set; }

        [JsonInclude]
        [JsonPropertyName("self-harm")]
        public bool SelfHarm { get; private set; }

        [JsonInclude]
        [JsonPropertyName("self-harm/intent")]
        public bool SelfHarmIntent { get; private set; }

        [JsonInclude]
        [JsonPropertyName("self-harm/instructions")]
        public bool SelfHarmInstructions { get; private set; }

        [JsonInclude]
        [JsonPropertyName("sexual")]
        public bool Sexual { get; private set; }

        [JsonInclude]
        [JsonPropertyName("sexual/minors")]
        public bool SexualMinors { get; private set; }

        [JsonInclude]
        [JsonPropertyName("violence")]
        public bool Violence { get; private set; }

        [JsonInclude]
        [JsonPropertyName("violence/graphic")]
        public bool ViolenceGraphic { get; private set; }
    }
}
