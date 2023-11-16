using System.Runtime.Serialization;

namespace OpenAI.Threads
{
    public enum RunStepToolCallType
    {
        [EnumMember(Value = "code_interpreter")]
        CodeInterpreter,
        [EnumMember(Value = "retrieval")]
        Retrieval,
        [EnumMember(Value = "function")]
        Function
    }
}