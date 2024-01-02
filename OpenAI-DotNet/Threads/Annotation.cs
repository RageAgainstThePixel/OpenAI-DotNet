// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class Annotation
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<AnnotationType>))]
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
        public FileCitation FileCitation { get; private set; }

        /// <summary>
        /// A URL for the file that's generated when the assistant used the 'code_interpreter' tool to generate a file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_path")]
        public FilePath FilePath { get; private set; }

        [JsonInclude]
        [JsonPropertyName("start_index")]
        public int StartIndex { get; private set; }

        [JsonInclude]
        [JsonPropertyName("end_index")]
        public int EndIndex { get; private set; }
    }
}