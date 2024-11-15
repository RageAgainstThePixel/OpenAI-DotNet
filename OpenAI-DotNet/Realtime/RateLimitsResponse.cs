// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class RateLimitsResponse : BaseRealtimeEvent, IServerEvent
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
        /// List of rate limit information.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("rate_limits")]
        public IReadOnlyList<RateLimit> RateLimits { get; private set; }
    }
}
