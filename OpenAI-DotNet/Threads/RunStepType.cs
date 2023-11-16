using System.Runtime.Serialization;

namespace OpenAI.Threads
{
    public enum RunStepType
    {
        [EnumMember(Value = "message_creation")]
        MessageCreation,
        [EnumMember(Value = "tool_calls")]
        ToolCalls
    }
}