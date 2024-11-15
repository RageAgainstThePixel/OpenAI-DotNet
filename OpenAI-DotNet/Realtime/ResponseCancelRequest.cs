// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    /// <summary>
    /// Send this event to cancel an in-progress response.
    /// The server will respond with a `response.cancelled` event or an error if there is no response to cancel.
    /// </summary>
    public sealed class ResponseCancelRequest : BaseRealtimeEvent, IClientEvent
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public override string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("type")]
        public override string Type { get; protected set; } = "response.cancel";
    }
}
