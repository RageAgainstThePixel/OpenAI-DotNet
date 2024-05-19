using System.Runtime.Serialization;

namespace OpenAI.Threads
{
    public enum TruncationStrategies
    {
        [EnumMember(Value = "auto")]
        Auto = 0,
        [EnumMember(Value = "last_messages")]
        LastMessages
    }
}