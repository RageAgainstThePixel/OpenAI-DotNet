using System.Runtime.Serialization;

namespace OpenAI.Assistants
{
    public enum AssistantToolType
    {
        [EnumMember(Value = "code_interpreter")]
        CodeInterpreter,
        [EnumMember(Value = "retrieval")]
        Retrieval,
        [EnumMember(Value = "function")]
        Function
    }
}