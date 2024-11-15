// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class VoiceActivityDetectionSettings
    {
        public VoiceActivityDetectionSettings() { }

        public VoiceActivityDetectionSettings(
            TurnDetectionType type = TurnDetectionType.Server_VAD,
            float? detectionThreshold = null,
            int? prefixPadding = null,
            int? silenceDuration = null)
        {
            switch (type)
            {
                case TurnDetectionType.Server_VAD:
                    Type = TurnDetectionType.Server_VAD;
                    DetectionThreshold = detectionThreshold;
                    PrefixPadding = prefixPadding;
                    SilenceDuration = silenceDuration;
                    break;
            }
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<TurnDetectionType>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public TurnDetectionType Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("threshold")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public float? DetectionThreshold { get; private set; }

        [JsonInclude]
        [JsonPropertyName("prefix_padding_ms")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? PrefixPadding { get; private set; }

        [JsonInclude]
        [JsonPropertyName("silence_duration_ms")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? SilenceDuration { get; private set; }

        public static VoiceActivityDetectionSettings Disabled() => new(TurnDetectionType.Disabled);
    }
}
