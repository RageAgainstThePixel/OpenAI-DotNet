// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    /// <summary>
    /// Send this event to truncate a previous assistant message’s audio.
    /// The server will produce audio faster than realtime,
    /// so this event is useful when the user interrupts to truncate audio
    /// that has already been sent to the client but not yet played.
    /// This will synchronize the server's understanding of the audio with the client's playback.
    /// Truncating audio will delete the server-side text transcript to ensure there
    /// is not text in the context that hasn't been heard by the user.
    /// If successful, the server will respond with a conversation.item.truncated event.
    /// </summary>
    public sealed class ConversationItemTruncateRequest : BaseRealtimeEvent, IClientEvent
    {
        public ConversationItemTruncateRequest(string itemId, int contentIndex, int audioEndMs)
        {
            ItemId = itemId;
            ContentIndex = contentIndex;
            AudioEndMs = audioEndMs;
        }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public override string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("type")]
        public override string Type { get; protected set; } = "conversation.item.truncate";

        /// <summary>
        /// The ID of the assistant message item to truncate. Only assistant message items can be truncated.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("item_id")]
        public string ItemId { get; private set; }

        /// <summary>
        /// The index of the content part to truncate. Set this to 0.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content_index")]
        public int ContentIndex { get; private set; }

        /// <summary>
        /// Inclusive duration up to which audio is truncated, in milliseconds.
        /// If the audio_end_ms is greater than the actual audio duration, the server will respond with an error.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("audio_end_ms")]
        public int AudioEndMs { get; private set; }
    }
}
