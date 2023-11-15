using System.Runtime.Serialization;

namespace OpenAI.ThreadMessages;

public enum RunStepToolCallType
{
    [EnumMember(Value = "code_interpreter")]
    CodeInterpreter,
    [EnumMember(Value = "retrieval")]
    Retrieval,
    [EnumMember(Value = "function")]
    Function
}