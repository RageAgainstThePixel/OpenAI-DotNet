namespace OpenAI.Realtime
{
    public interface IVoiceActivityDetectionSettings
    {
        public TurnDetectionType Type { get; }
        public bool? CreateResponse { get; }
        public bool? InterruptResponse { get; }
    }
}
