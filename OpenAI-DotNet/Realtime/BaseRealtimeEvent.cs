// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;

namespace OpenAI.Realtime
{
    public abstract class BaseRealtimeEvent : IRealtimeEvent
    {
        /// <inheritdoc />
        public abstract string EventId { get; internal set; }

        /// <inheritdoc />
        public abstract string Type { get; protected set; }

        /// <inheritdoc />
        public string ToJsonString()
            => this.ToEscapedJsonString<object>();
    }
}
