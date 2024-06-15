// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class TextContent : IAppendable<TextContent>
    {
        public TextContent() { }

        public TextContent(string value, IEnumerable<Annotation> annotations = null)
        {
            Value = value;
            this.annotations = annotations?.ToList();
        }

        [JsonInclude]
        [JsonPropertyName("index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Index { get; private set; }

        /// <summary>
        /// The data that makes up the text.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Value { get; private set; }

        private List<Annotation> annotations;

        /// <summary>
        /// Annotations
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("annotations")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<Annotation> Annotations
        {
            get => annotations;
            private set => annotations = value?.ToList();
        }

        public static implicit operator TextContent(string value) => new(value);

        public static implicit operator string(TextContent content) => content.Value;

        public override string ToString() => Value;

        public void AppendFrom(TextContent other)
        {
            if (other == null) { return; }

            if (other.Index.HasValue)
            {
                Index = other.Index.Value;
            }

            if (!string.IsNullOrWhiteSpace(other.Value))
            {
                Value += other.Value;
            }

            if (other is { Annotations: not null })
            {
                annotations ??= new List<Annotation>();
                annotations.AppendFrom(other.Annotations);
            }
        }
    }
}
