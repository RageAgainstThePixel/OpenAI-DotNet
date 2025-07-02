// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Realtime;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI
{
    internal class VoiceActivityDetectionSettingsConverter : JsonConverter<IVoiceActivityDetectionSettings>
    {
        public override IVoiceActivityDetectionSettings Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var root = JsonDocument.ParseValue(ref reader).RootElement;
            var type = root.GetProperty("type").GetString() ?? "disabled";

            return type switch
            {
                "disabled" => new DisabledVAD(),
                "server_vad" => root.Deserialize<ServerVAD>(options) ?? new ServerVAD(),
                "semantic_vad" => root.Deserialize<SemanticVAD>(options) ?? new SemanticVAD(),
                _ => throw new NotImplementedException($"Unknown VAD type: {type}")
            };
        }

        public override void Write(Utf8JsonWriter writer, IVoiceActivityDetectionSettings value, JsonSerializerOptions options)
        {
            switch (value.Type)
            {
                case TurnDetectionType.Disabled:
                    writer.WriteNullValue();
                    break;
                default:
                    JsonSerializer.Serialize(writer, value, value.GetType(), options);
                    break;
            }
        }
    }
}
