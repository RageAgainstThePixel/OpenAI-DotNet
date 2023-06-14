using System.Runtime.Serialization;

namespace OpenAI.Chat
{
    public enum Role
    {
        [EnumMember(Value = "system")]
        System = 1,
        [EnumMember(Value = "assistant")]
        Assistant = 2,
        [EnumMember(Value = "user")]
        User = 3,
        [EnumMember(Value = "function")]
        Function = 4,
    }
}