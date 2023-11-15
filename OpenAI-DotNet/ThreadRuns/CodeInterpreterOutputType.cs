using System.Runtime.Serialization;

namespace OpenAI.ThreadMessages;

public enum CodeInterpreterOutputType
{
    [EnumMember(Value = "logs")]
    Logs,
    [EnumMember(Value = "image")]
    Image
}