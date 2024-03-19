// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Audio
{
    /// <summary>
    /// Segment of the transcribed text and their corresponding details.
    /// </summary>
    public sealed class TranscriptionSegment
    {
        /// <summary>
        /// Unique identifier of the segment.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public int Id { get; private set; }

        /// <summary>
        /// Seek offset of the segment.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("seek")]
        public int Seek { get; private set; }

        /// <summary>
        /// Start time of the segment in seconds.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("start")]
        public double Start { get; private set; }

        /// <summary>
        /// End time of the segment in seconds.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("end")]
        public double End { get; private set; }

        /// <summary>
        /// Text content of the segment.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        public string Text { get; private set; }

        /// <summary>
        /// Array of token IDs for the text content.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tokens")]
        public int[] Tokens { get; private set; }

        /// <summary>
        /// Temperature parameter used for generating the segment.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("temperature")]
        public double Temperature { get; private set; }

        /// <summary>
        /// Average logprob of the segment.
        /// If the value is lower than -1, consider the logprobs failed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("avg_logprob")]
        public double AverageLogProbability { get; private set; }

        /// <summary>
        /// Compression ratio of the segment.
        /// If the value is greater than 2.4, consider the compression failed.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("compression_ratio")]
        public double CompressionRatio { get; private set; }

        /// <summary>
        /// Probability of no speech in the segment.
        /// If the value is higher than 1.0 and the avg_logprob is below -1, consider this segment silent.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("no_speech_prob")]
        public double NoSpeechProbability { get; private set; }
    }
}