// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public interface IVoiceActivityDetectionSettings
    {
        public TurnDetectionType Type { get; }
        public bool CreateResponse { get; }
        public bool InterruptResponse { get; }
    }

    public sealed class DisabledVAD : IVoiceActivityDetectionSettings
    {
        public TurnDetectionType Type => TurnDetectionType.Disabled;

        public bool CreateResponse => false;

        public bool InterruptResponse => false;
    }

    public enum VAD_Eagerness
    {
        [EnumMember(Value = "auto")]
        Auto = 0,
        [EnumMember(Value = "low")]
        Low,
        [EnumMember(Value = "medium")]
        Medium,
        [EnumMember(Value = "high")]
        High
    }

    public sealed class SemanticVAD : IVoiceActivityDetectionSettings
    {
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

    public sealed class ServerVAD : IVoiceActivityDetectionSettings
    {
        public ServerVAD(
            bool createResponse = true,
            bool interruptResponse = true,
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
        public bool CreateResponse { get; private set; }

        [JsonInclude]
        [JsonPropertyName("interrupt_response")]
        public bool InterruptResponse { get; private set; }

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

    [Obsolete("Use new IVoiceActivityDetectionSettings classes: SemanticVAD, ServerVAD, and DisabledVAD")]
    public sealed class VoiceActivityDetectionSettings : IVoiceActivityDetectionSettings
    {
        public VoiceActivityDetectionSettings() { }

        public VoiceActivityDetectionSettings(
            TurnDetectionType type = TurnDetectionType.Server_VAD,
            float? detectionThreshold = null,
            int? prefixPadding = null,
            int? silenceDuration = null,
            bool createResponse = true)
        {
            switch (type)
            {
                default:
                case TurnDetectionType.Server_VAD:
                    Type = TurnDetectionType.Server_VAD;
                    DetectionThreshold = detectionThreshold;
                    PrefixPadding = prefixPadding;
                    SilenceDuration = silenceDuration;
                    CreateResponse = createResponse;
                    break;
                case TurnDetectionType.Disabled:
                    Type = TurnDetectionType.Disabled;
                    DetectionThreshold = null;
                    PrefixPadding = null;
                    SilenceDuration = null;
                    CreateResponse = false;
                    break;
            }
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(Extensions.JsonStringEnumConverter<TurnDetectionType>))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public TurnDetectionType Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("create_response")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool CreateResponse { get; private set; }

        [JsonInclude]
        [JsonPropertyName("interrupt_response")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool InterruptResponse { get; private set; }

        [JsonInclude]
        [JsonPropertyName("threshold")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public float? DetectionThreshold { get; private set; }

        [JsonInclude]
        [JsonPropertyName("prefix_padding_ms")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PrefixPadding { get; private set; }

        [JsonInclude]
        [JsonPropertyName("silence_duration_ms")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? SilenceDuration { get; private set; }

        public static VoiceActivityDetectionSettings Disabled() => new(TurnDetectionType.Disabled);
    }
}
