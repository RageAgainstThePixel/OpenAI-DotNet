// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class FileCitation : IAnnotation
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<AnnotationType>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public AnnotationType Type { get; private set; } = AnnotationType.FileCitation;

        /// <summary>
        /// The ID of the file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_id")]
        public string FileId { get; private set; }

        /// <summary>
        /// The filename of the file cited.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("filename")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string FileName { get; private set; }

        /// <summary>
        /// The index of the file in the list of files.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Index { get; private set; }
    }
}
