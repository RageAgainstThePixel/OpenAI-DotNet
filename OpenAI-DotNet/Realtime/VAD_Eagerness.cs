using System.Runtime.Serialization;

namespace OpenAI.Realtime
{
    public enum VAD_Eagerness
    {
        [EnumMember(Value = "auto")]
        Auto = 0,
        [EnumMember(Value = "low")]
        Low,
        [EnumMember(Value = "medium")]
        Medium,
        [EnumMember(Value = "high")]
        High
    }
}