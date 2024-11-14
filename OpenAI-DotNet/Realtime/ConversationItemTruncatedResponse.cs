// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ConversationItemTruncatedResponse : BaseRealtimeEvent, IServerEvent
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
        /// The ID of the assistant message item that was truncated.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("item_id")]
        public string ItemId { get; private set; }

        /// <summary>
        /// The index of the content part that was truncated.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content_index")]
        public int ContentIndex { get; private set; }

        /// <summary>
        /// The duration up to which the audio was truncated, in milliseconds.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("audio_end_ms")]
        public int AudioEndMs { get; private set; }
    }
}
