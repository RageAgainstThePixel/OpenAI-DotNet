// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Audio
{
    public sealed class AudioResponse
    {
        /// <summary>
        /// The language of the input audio.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("language")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Language { get; private set; }

        /// <summary>
        /// The duration of the input audio.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("duration")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public double? Duration { get; private set; }

        /// <summary>
        /// The transcribed text.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        public string Text { get; private set; }

        /// <summary>
        /// Extracted words and their corresponding timestamps.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("words")]
        public TranscriptionWord[] Words { get; private set; }

        /// <summary>
        /// Segments of the transcribed text and their corresponding details.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("segments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public TranscriptionSegment[] Segments { get; private set; }
    }
}