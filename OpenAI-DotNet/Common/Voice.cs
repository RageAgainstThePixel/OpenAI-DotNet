// Licensed under the MIT License. See LICENSE in the project root for license information.


namespace OpenAI
{
    public class Voice
    {
        public Voice(string id) { Id = id; }

        public string Id { get; }

        public override string ToString() => Id;

        public static implicit operator string(Voice voice) => voice?.ToString();

        public static implicit operator Voice(string id) => new(id);

        public static readonly Voice Alloy = new("alloy");
        public static readonly Voice Ash = new("ash");
        public static readonly Voice Ballad = new("ballad");
        public static readonly Voice Coral = new("coral");
        public static readonly Voice Echo = new("echo");
        public static readonly Voice Fable = new("fable");
        public static readonly Voice Onyx = new("onyx");
        public static readonly Voice Nova = new("nova");
        public static readonly Voice Sage = new("sage");
        public static readonly Voice Shimmer = new("shimmer");
        public static readonly Voice Verse = new("verse");

#pragma warning disable CS0618 // Type or member is obsolete
        public static implicit operator Voice(Audio.SpeechVoice voice)
        {
            return voice switch
            {
                Audio.SpeechVoice.Alloy => Alloy,
                Audio.SpeechVoice.Echo => Echo,
                Audio.SpeechVoice.Fable => Fable,
                Audio.SpeechVoice.Onyx => Onyx,
                Audio.SpeechVoice.Nova => Nova,
                Audio.SpeechVoice.Shimmer => Shimmer,
                _ => null
            };
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
