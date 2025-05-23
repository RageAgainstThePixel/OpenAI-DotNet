namespace OpenAI.Realtime
{
    public sealed class DisabledVAD : IVoiceActivityDetectionSettings
    {
        public TurnDetectionType Type => TurnDetectionType.Disabled;

        public bool CreateResponse => false;

        public bool InterruptResponse => false;
    }
}