// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ConversationItemCreatedResponse : BaseRealtimeEvent, IServerEvent
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
        /// The ID of the preceding item.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("previous_item_id")]
        public string PreviousItemId { get; private set; }

        /// <summary>
        /// The item that was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("item")]
        public ConversationItem Item { get; private set; }
    }
}
