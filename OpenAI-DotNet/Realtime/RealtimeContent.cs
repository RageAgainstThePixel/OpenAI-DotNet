// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class RealtimeContent
    {
        public RealtimeContent() { }

        public RealtimeContent(string text, RealtimeContentType type)
        {
            Type = type;
            Text = type switch
            {
                RealtimeContentType.InputText or RealtimeContentType.Text => text,
                _ => throw new ArgumentException($"Invalid content type {type} for text content")
            };
        }

        public RealtimeContent(ReadOnlyMemory<byte> audioData, RealtimeContentType type, string transcript = null)
            : this(audioData.Span, type, transcript)
        {
        }

        public RealtimeContent(ReadOnlySpan<byte> audioData, RealtimeContentType type, string transcript = null)
        {
            Type = type;
            Audio = type switch
            {
                RealtimeContentType.InputAudio or RealtimeContentType.Audio => Convert.ToBase64String(audioData),
                _ => throw new ArgumentException($"Invalid content type {type} for audio content")
            };
            Transcript = transcript;
        }

        public RealtimeContent(byte[] audioData, RealtimeContentType type, string transcript = null)
        {
            Type = type;
            Audio = type switch
            {
                RealtimeContentType.InputAudio or RealtimeContentType.Audio => Convert.ToBase64String(audioData),
                _ => throw new ArgumentException($"Invalid content type {type} for audio content")
            };
            Transcript = transcript;
        }

        /// <summary>
        /// The content type ("text", "audio", "input_text", "input_audio").
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<RealtimeContentType>))]
        public RealtimeContentType Type { get; private set; }

        /// <summary>
        /// The text content.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Text { get; private set; }

        /// <summary>
        /// Base64-encoded audio data.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("audio")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Audio { get; private set; }

        /// <summary>
        /// The transcript of the audio.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("transcript")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Transcript { get; private set; }

        public static implicit operator RealtimeContent(string text) => new(text, RealtimeContentType.InputText);

        public static implicit operator RealtimeContent(byte[] audioData) => new(audioData, RealtimeContentType.InputAudio);
    }
}
