// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Realtime;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class VoiceActivityDetectionSettingsConverter : JsonConverter<VoiceActivityDetectionSettings>
    {
        public override VoiceActivityDetectionSettings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType == JsonTokenType.Null
                ? VoiceActivityDetectionSettings.Disabled()
                : JsonSerializer.Deserialize<VoiceActivityDetectionSettings>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, VoiceActivityDetectionSettings value, JsonSerializerOptions options)
        {
            switch (value.Type)
            {
                case TurnDetectionType.Disabled:
                    writer.WriteNullValue();
                    break;
                default:
                case TurnDetectionType.Server_VAD:
                    JsonSerializer.Serialize(writer, value, options);
                    break;
            }
        }
    }
}
