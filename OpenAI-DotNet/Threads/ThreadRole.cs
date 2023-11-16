using System.Runtime.Serialization;

namespace OpenAI.Threads
{
    public enum ThreadRole
    {
        [EnumMember(Value = "user")]
        User,

        [EnumMember(Value = "assistant")]
        Assistant
    }
}