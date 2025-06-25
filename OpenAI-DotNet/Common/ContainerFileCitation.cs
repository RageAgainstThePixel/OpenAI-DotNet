// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ContainerFileCitation : IAnnotation
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<AnnotationType>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public AnnotationType Type { get; private set; } = AnnotationType.ContainerFileCitation;

        /// <summary>
        /// The ID of the container file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("container_id")]
        public string ContainerId { get; private set; }

        /// <summary>
        /// The ID of the file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_id")]
        public string FileId { get; private set; }

        /// <summary>
        /// The filename of the container file cited.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("filename")]
        public string FileName { get; private set; }

        /// <summary>
        /// The index of the first character of the container file citation in the message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("start_index")]
        public int StartIndex { get; private set; }

        /// <summary>
        /// The index of the last character of the container file citation in the message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("end_index")]
        public int EndIndex { get; private set; }
    }
}
