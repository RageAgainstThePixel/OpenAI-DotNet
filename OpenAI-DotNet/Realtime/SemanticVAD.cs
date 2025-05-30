using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class SemanticVAD : IVoiceActivityDetectionSettings
    {
        public SemanticVAD() { }

        public SemanticVAD(bool createResponse = true, bool interruptResponse = true, VAD_Eagerness eagerness = VAD_Eagerness.Auto)
        {
            CreateResponse = createResponse;
            InterruptResponse = interruptResponse;
            Eagerness = eagerness;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<TurnDetectionType>))]
        public TurnDetectionType Type { get; private set; } = TurnDetectionType.Semantic_VAD;

        [JsonInclude]
        [JsonPropertyName("create_response")]
        public bool CreateResponse { get; private set; }

        [JsonInclude]
        [JsonPropertyName("interrupt_response")]
        public bool InterruptResponse { get; private set; }

        [JsonInclude]
        [JsonPropertyName("eagerness")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<VAD_Eagerness>))]
        public VAD_Eagerness Eagerness { get; private set; }
    }
}
