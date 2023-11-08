using System.Runtime.Serialization;

namespace OpenAI.Chat
{
    public enum ChatResponseFormat
    {
        [EnumMember(Value = "text")]
        Text,
        [EnumMember(Value = "json_object")]
        Json
    }
}