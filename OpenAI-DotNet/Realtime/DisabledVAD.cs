namespace OpenAI.Realtime
{
    public sealed class DisabledVAD : IVoiceActivityDetectionSettings
    {
        public TurnDetectionType Type => TurnDetectionType.Disabled;

        public bool? CreateResponse => null;

        public bool? InterruptResponse => null;
    }
}
