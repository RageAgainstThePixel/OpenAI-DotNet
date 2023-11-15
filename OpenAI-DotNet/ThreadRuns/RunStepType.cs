using System.Runtime.Serialization;

namespace OpenAI.ThreadRuns;

public enum RunStepType
{
    [EnumMember(Value = "message_creation")]
    MessageCreation,
    [EnumMember(Value = "tool_calls")]
    ToolCalls
}