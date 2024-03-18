// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Audio
{
    /// <summary>
    /// Extracted word and their corresponding timestamps.
    /// </summary>
    public sealed class TranscriptionWord
    {
        /// <summary>
        /// The text content of the word.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("word")]
        public string Word { get; private set; }

        /// <summary>
        /// Start time of the word in seconds.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("start")]
        public double Start { get; private set; }

        /// <summary>
        /// End time of the word in seconds.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("end")]
        public double End { get; private set; }
    }
}