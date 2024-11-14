// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Realtime;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class RealtimeClientEventConverter : JsonConverter<IRealtimeEvent>
    {
        public override void Write(Utf8JsonWriter writer, IRealtimeEvent value, JsonSerializerOptions options)
            => throw new NotImplementedException();

        public override IRealtimeEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var root = JsonDocument.ParseValue(ref reader).RootElement;
            var type = root.GetProperty("type").GetString();

            return type switch
            {
                "session.update" => root.Deserialize<UpdateSessionRequest>(options),
                "input_audio_buffer.append" => root.Deserialize<InputAudioBufferAppendRequest>(options),
                "input_audio_buffer.commit" => root.Deserialize<InputAudioBufferCommitRequest>(options),
                "input_audio_buffer.clear" => root.Deserialize<InputAudioBufferClearRequest>(options),
                "conversation.item.create" => root.Deserialize<ConversationItemCreateRequest>(options),
                "conversation.item.truncate" => root.Deserialize<ConversationItemTruncateRequest>(options),
                "conversation.item.delete" => root.Deserialize<ConversationItemDeleteRequest>(options),
                "response.create" => root.Deserialize<CreateResponseRequest>(options),
                "response.cancel" => root.Deserialize<ResponseCancelRequest>(options),
                _ => throw new NotImplementedException($"Unknown {nameof(IRealtimeEvent)}: {type}")
            };
        }
    }
}
