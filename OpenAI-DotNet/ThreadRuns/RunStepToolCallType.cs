using System.Runtime.Serialization;

namespace OpenAI.ThreadRuns;

public enum RunStepToolCallType
{
    [EnumMember(Value = "code_interpreter")]
    CodeInterpreter,
    [EnumMember(Value = "retrieval")]
    Retrieval,
    [EnumMember(Value = "function")]
    Function
}