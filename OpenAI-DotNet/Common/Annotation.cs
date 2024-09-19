// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Annotation : IAppendable<Annotation>
    {
        [JsonInclude]
        [JsonPropertyName("index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? Index { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<AnnotationType>))]
        public AnnotationType Type { get; private set; }

        /// <summary>
        /// The text in the message content that needs to be replaced.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        public string Text { get; private set; }

        /// <summary>
        /// A citation within the message that points to a specific quote from a
        /// specific File associated with the assistant or the message.
        /// Generated when the assistant uses the 'retrieval' tool to search files.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_citation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FileCitation FileCitation { get; private set; }

        /// <summary>
        /// A URL for the file that's generated when the assistant used the 'code_interpreter' tool to generate a file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_path")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FilePath FilePath { get; private set; }

        [JsonInclude]
        [JsonPropertyName("start_index")]
        public int StartIndex { get; private set; }

        [JsonInclude]
        [JsonPropertyName("end_index")]
        public int EndIndex { get; private set; }

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
