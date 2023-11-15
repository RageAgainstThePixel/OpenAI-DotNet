using System.Runtime.Serialization;

namespace OpenAI.ThreadMessages;

public enum RunStepType
{
    [EnumMember(Value = "message_creation")]
    MessageCreation,
    [EnumMember(Value = "tool_calls")]
    ToolCalls
}