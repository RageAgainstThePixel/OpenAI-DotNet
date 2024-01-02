// Licensed under the MIT License. See LICENSE in the project root for license information.

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