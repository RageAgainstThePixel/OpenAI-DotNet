using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ServerVAD : IVoiceActivityDetectionSettings
    {
        public ServerVAD() { }

        public ServerVAD(
            bool? createResponse = true,
            bool? interruptResponse = true,
            int? prefixPadding = null,
            int? silenceDuration = null,
            float? detectionThreshold = null)
        {
            CreateResponse = createResponse;
            InterruptResponse = interruptResponse;
            PrefixPadding = prefixPadding;
            SilenceDuration = silenceDuration;
            DetectionThreshold = detectionThreshold;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<TurnDetectionType>))]
        public TurnDetectionType Type { get; private set; } = TurnDetectionType.Server_VAD;

        [JsonInclude]
        [JsonPropertyName("create_response")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? CreateResponse { get; private set; }

        [JsonInclude]
        [JsonPropertyName("interrupt_response")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? InterruptResponse { get; private set; }

        [JsonInclude]
        [JsonPropertyName("prefix_padding_ms")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PrefixPadding { get; private set; }

        [JsonInclude]
        [JsonPropertyName("silence_duration_ms")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SilenceDuration { get; private set; }

        [JsonInclude]
        [JsonPropertyName("threshold")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? DetectionThreshold { get; private set; }
    }
}
