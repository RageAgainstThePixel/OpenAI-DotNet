using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ContentText
    {
        public ContentText(string value) => Value = value;

        public static implicit operator string(ContentText text) => text.Value;

        public static implicit operator ContentText(string value) => new ContentText(value);

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