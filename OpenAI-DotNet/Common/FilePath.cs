// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class FilePath : IAnnotation
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<AnnotationType>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public AnnotationType Type { get; private set; } = AnnotationType.FilePath;

        /// <summary>
        /// The ID of the file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_id")]
        public string FileId { get; private set; }

        /// <summary>
        /// The MIME type of the file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("mime_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string MimeType { get; private set; }

        /// <summary>
        /// The index of the file in the list of files.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("index")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Index { get; private set; }
    }
}
