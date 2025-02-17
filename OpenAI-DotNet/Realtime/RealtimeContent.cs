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
            switch (type)
            {
                case RealtimeContentType.InputText or RealtimeContentType.Text:
                    Text = text;
                    break;
                case RealtimeContentType.InputAudio or RealtimeContentType.Audio:
                    Audio = text;
                    break;
                case RealtimeContentType.ItemReference:
                    Id = text;
                    break;
                default:
                    throw new ArgumentException($"Invalid content type {type} for text content");
            }
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
        /// ID of a previous conversation item to reference (for `item_reference` content types in `response.create` events).
        /// These can reference both client and server created items.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Id { get; private set; }

        /// <summary>
        /// The content type (`input_text`, `input_audio`, `item_reference`, `text`).
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<RealtimeContentType>))]
        public RealtimeContentType Type { get; private set; }

        /// <summary>
        /// The text content, used for `input_text` and `text` content types.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("text")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Text { get; private set; }

        /// <summary>
        /// Base64-encoded audio bytes, used for `input_audio` content type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("audio")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Audio { get; private set; }

        /// <summary>
        /// The transcript of the audio, used for `input_audio` content type.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("transcript")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Transcript { get; private set; }

        public static implicit operator RealtimeContent(string text) => new(text, RealtimeContentType.InputText);

        public static implicit operator RealtimeContent(byte[] audioData) => new(audioData, RealtimeContentType.InputAudio);
    }
}
