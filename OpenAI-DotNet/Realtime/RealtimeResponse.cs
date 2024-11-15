// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class RealtimeResponse : BaseRealtimeEvent, IServerEvent
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
        /// The response resource.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("response")]
        public RealtimeResponseResource Response { get; private set; }
    }
}
