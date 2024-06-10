// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Batch
{
    public enum BatchStatus
    {
        NotStarted = 0,
        [EnumMember(Value = "validating")]
        Validating,
        [EnumMember(Value = "in_progress")]
        InProgress,
        [EnumMember(Value = "finalizing")]
        Finalizing,
        [EnumMember(Value = "cancelling")]
        Cancelling,
        [EnumMember(Value = "cancelled")]
        Cancelled,
        [EnumMember(Value = "completed")]
        Completed,
        [EnumMember(Value = "expired")]
        Expired,
        [EnumMember(Value = "failed")]
        Failed,
    }
}
