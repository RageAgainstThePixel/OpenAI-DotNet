// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ConversationItemInputAudioTranscriptionResponse : BaseRealtimeEvent, IServerEvent
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public override string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("type")]
        public override string Type { get; protected set; }

        /// <summary>
        /// The ID of the user message item.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("item_id")]
        public string ItemId { get; private set; }

        /// <summary>
        /// The index of the content part containing the audio.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content_index")]
        public int? ContentIndex { get; private set; }

        /// <summary>
        /// The transcribed text.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("transcript")]
        public string Transcript { get; private set; }

        /// <summary>
        /// Details of the transcription error.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("error")]
        public Error Error { get; private set; }

        [JsonIgnore]
        public bool IsCompleted => Type.Contains("completed");

        [JsonIgnore]
        public bool IsFailed => Type.Contains("failed");
    }
}
