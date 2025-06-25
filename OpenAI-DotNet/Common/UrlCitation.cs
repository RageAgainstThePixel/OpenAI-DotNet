// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class UrlCitation : IAnnotation
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<AnnotationType>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public AnnotationType Type { get; private set; } = AnnotationType.UrlCitation;

        /// <summary>
        /// The URL of the web resource.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("url")]
        public string Url { get; private set; }

        /// <summary>
        /// The title of the web resource.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("title")]
        public string Title { get; private set; }

        /// <summary>
        /// The index of the last character of the URL citation in the message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("start_index")]
        public int StartIndex { get; private set; }

        /// <summary>
        /// The index of the first character of the URL citation in the message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("end_index")]
        public int EndIndex { get; private set; }
    }
}
