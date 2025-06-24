// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Annotation : IAppendable<Annotation>
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<AnnotationType>))]
        public AnnotationType Type { get; private set; }

        /// <summary>
        /// The text in the message content that needs to be replaced.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Text { get; private set; }

        /// <summary>
        /// A citation to a file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_citation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FileCitation FileCitation { get; private set; }

        /// <summary>
        /// A path to a file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_path")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FilePath FilePath { get; private set; }

        [JsonInclude]
        [JsonPropertyName("index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Index { get; private set; }

        [JsonInclude]
        [JsonPropertyName("start_index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? StartIndex { get; private set; }

        [JsonInclude]
        [JsonPropertyName("end_index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? EndIndex { get; private set; }

        public void AppendFrom(Annotation other)
        {
            if (other == null) { return; }

            if (other.Index.HasValue)
            {
                Index = other.Index.Value;
            }

            if (!string.IsNullOrWhiteSpace(other.Text))
            {
                Text += other.Text;
            }

            if (other.FileCitation != null)
            {
                FileCitation = other.FileCitation;
            }

            if (other.FilePath != null)
            {
                FilePath = other.FilePath;
            }

            if (other.StartIndex > 0)
            {
                StartIndex = other.StartIndex;
            }

            if (other.EndIndex > 0)
            {
                EndIndex = other.EndIndex;
            }
        }
    }
}
