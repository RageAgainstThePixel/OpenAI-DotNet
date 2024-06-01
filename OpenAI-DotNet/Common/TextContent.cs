// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public class TextContent
    {
        public TextContent(string value, IEnumerable<string> annotations = null)
        {
            Value = value;
            Annotations = annotations?.ToList();
        }

        [JsonInclude]
        [JsonPropertyName("value")]
        public string Value { get; private set; }

        [JsonInclude]
        [JsonPropertyName("annotations")]
        public IReadOnlyList<string> Annotations { get; private set; }

        public static implicit operator TextContent(string value) => new(value);

        public static implicit operator string(TextContent content) => content.Value;

        public override string ToString() => Value;
    }
}
