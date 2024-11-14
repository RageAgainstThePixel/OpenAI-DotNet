// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class RealtimeConversationResponse : BaseRealtimeEvent, IServerEvent
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
        /// The conversation resource.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("conversation")]
        public RealtimeConversation Conversation { get; private set; }

        public static implicit operator RealtimeConversation(RealtimeConversationResponse response) => response?.Conversation;
    }
}
