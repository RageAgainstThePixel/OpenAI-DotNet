// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class RealtimeEventError : BaseRealtimeEvent, IServerEvent
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public override string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("type")]
        public override string Type { get; protected set; }

        [JsonInclude]
        [JsonPropertyName("error")]
        public Error Error { get; private set; }

        public override string ToString()
            => Error.ToString();

        public static implicit operator Exception(RealtimeEventError error)
            => error.Error?.Exception ?? new Exception(error.ToString());
    }
}
