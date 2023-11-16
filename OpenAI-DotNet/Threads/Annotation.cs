using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.Threads
{
    public sealed class Annotation
    {
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<AnnotationType>))]
        public AnnotationType Type { get; private set; }

        /// <summary>
        /// The text in the message content that needs to be replaced.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; private set; }

        /// <summary>
        /// A citation within the message that points to a specific quote from a specific File associated with the assistant or the message.
        /// Generated when the assistant uses the "retrieval" tool to search files.
        /// </summary>
        [JsonPropertyName("file_citation")]
        public FileCitation FileCitation { get; private set; }

        /// <summary>
        /// A URL for the file that's generated when the assistant used the code_interpreter tool to generate a file.
        /// </summary>
        [JsonPropertyName("file_path")]
        public FilePath FilePath { get; private set; }

        [JsonPropertyName("start_index")]
        public int StartIndex { get; private set; }

        [JsonPropertyName("end_index")]
        public int EndIndex { get; private set; }
    }
}