using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class TextContent
    {
        /// <summary>
        /// The data that makes up the text.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("value")]
        public string Value { get; private set; }

        /// <summary>
        /// Annotations
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("annotations")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyList<Annotation> Annotations { get; private set; }
    }
}