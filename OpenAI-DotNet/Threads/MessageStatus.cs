// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Threads
{
    public enum MessageStatus
    {
        NotStarted = 0,
        [EnumMember(Value = "in_progress")]
        InProgress,
        [EnumMember(Value = "incomplete")]
        Incomplete,
        [EnumMember(Value = "completed")]
        Completed
    }
}
