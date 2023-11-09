using System.Runtime.Serialization;

namespace OpenAI.Chat
{
    public enum ContentType
    {
        [EnumMember(Value = "text")]
        Text,
        [EnumMember(Value = "image_url")]
        ImageUrl
    }
}