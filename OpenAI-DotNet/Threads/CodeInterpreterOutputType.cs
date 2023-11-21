using System.Runtime.Serialization;

namespace OpenAI.Threads
{
    public enum CodeInterpreterOutputType
    {
        [EnumMember(Value = "logs")]
        Logs,
        [EnumMember(Value = "image")]
        Image
    }
}