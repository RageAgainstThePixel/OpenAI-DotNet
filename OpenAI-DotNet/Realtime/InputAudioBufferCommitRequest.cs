// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    /// <summary>
    /// Send this event to commit the user input audio buffer,
    /// which will create a new user message item in the conversation.
    /// This event will produce an error if the input audio buffer is empty.
    /// When in Server VAD mode, the client does not need to send this event,
    /// the server will commit the audio buffer automatically.
    /// Committing the input audio buffer will trigger input audio transcription (if enabled in session configuration),
    /// but it will not create a response from the model.
    /// The server will respond with an input_audio_buffer.committed event.
    /// </summary>
    public sealed class InputAudioBufferCommitRequest : BaseRealtimeEvent, IClientEvent
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public override string EventId { get; internal set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonPropertyName("type")]
        public override string Type { get; protected set; } = "input_audio_buffer.commit";
    }
}
