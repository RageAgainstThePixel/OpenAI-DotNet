// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    /// <summary>
    /// Send this event to clear the audio bytes in the buffer.
    /// The server will respond with an input_audio_buffer.cleared event.
    /// </summary>
    public sealed class InputAudioBufferClearRequest : BaseRealtimeEvent, IClientEvent
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public override string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("type")]
        public override string Type { get; protected set; } = "input_audio_buffer.clear";
    }
}
