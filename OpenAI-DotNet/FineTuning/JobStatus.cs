// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.FineTuning
{
    public enum JobStatus
    {
        NotStarted = 0,
        [EnumMember(Value = "validating_files")]
        ValidatingFiles,
        [EnumMember(Value = "queued")]
        Queued,
        [EnumMember(Value = "running")]
        Running,
        [EnumMember(Value = "succeeded")]
        Succeeded,
        [EnumMember(Value = "failed")]
        Failed,
        [EnumMember(Value = "cancelled")]
        Cancelled
    }
}
