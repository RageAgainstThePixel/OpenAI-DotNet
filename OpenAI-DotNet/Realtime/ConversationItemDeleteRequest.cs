// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    /// <summary>
    /// Send this event when you want to remove any item from the conversation history.
    /// The server will respond with a conversation.item.deleted event,
    /// unless the item does not exist in the conversation history,
    /// in which case the server will respond with an error.
    /// </summary>
    public sealed class ConversationItemDeleteRequest : BaseRealtimeEvent, IClientEvent
    {
        public ConversationItemDeleteRequest() { }

        public ConversationItemDeleteRequest(string itemId)
        {
            ItemId = itemId;
        }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public override string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("type")]
        public override string Type { get; protected set; } = "conversation.item.delete";

        /// <summary>
        /// The ID of the item to delete.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("item_id")]
        public string ItemId { get; private set; }
    }
}
