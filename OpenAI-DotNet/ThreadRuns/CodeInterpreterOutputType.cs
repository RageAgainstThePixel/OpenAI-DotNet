using System.Runtime.Serialization;

namespace OpenAI.ThreadRuns
{
    public enum CodeInterpreterOutputType
    {
        [EnumMember(Value = "logs")]
        Logs,
        [EnumMember(Value = "image")]
        Image
    }
}