// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class TextContent
    {
        public TextContent() { }

        public TextContent(string value, IEnumerable<Annotation> annotations = null)
        {
            Value = value;
            Annotations = annotations?.ToList();
        }

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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<Annotation> Annotations { get; private set; }

        public static implicit operator TextContent(string value) => new(value);

        public static implicit operator string(TextContent content) => content.Value;

        public override string ToString() => Value;
    }
}
