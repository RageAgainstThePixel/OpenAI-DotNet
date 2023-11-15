using System.Runtime.Serialization;

namespace OpenAI.ThreadMessages;

public enum StepDetailsType
{
    [EnumMember(Value = "message_creation")]
    MessageCreation,
    [EnumMember(Value = "tool_calls")]
    ToolCalls
        
}