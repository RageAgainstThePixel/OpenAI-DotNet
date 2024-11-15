// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ResponseOutputItemResponse : BaseRealtimeEvent, IServerEvent
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
        /// The ID of the response to which the item belongs.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("response_id")]
        public string ResponseId { get; private set; }

        /// <summary>
        /// The index of the output item in the response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("output_index")]
        public int OutputIndex { get; private set; }

        /// <summary>
        /// The item that was added.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("item")]
        public ConversationItem Item { get; private set; }
    }
}
