using System.Runtime.Serialization;

namespace OpenAI.ThreadMessages;

public enum ThreadRole
{
    [EnumMember(Value = "user")]
    User,

    [EnumMember(Value = "assistant")]
    Assistant
}