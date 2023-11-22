using System.Runtime.Serialization;

namespace OpenAI
{
    public enum ImageDetail
    {
        [EnumMember(Value = "auto")]
        Auto,
        [EnumMember(Value = "low")]
        Low,
        [EnumMember(Value = "high")]
        High
    }
}