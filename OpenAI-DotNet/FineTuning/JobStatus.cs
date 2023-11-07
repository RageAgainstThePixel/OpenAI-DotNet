using System.Runtime.Serialization;

namespace OpenAI.FineTuning
{
    public enum JobStatus
    {
        NotStarted = 0,
        [EnumMember(Value = "validating_files")]
        ValidatingFiles = 1,
        [EnumMember(Value = "queued")]
        Queued = 2,
        [EnumMember(Value = "running")]
        Running = 3,
        [EnumMember(Value = "succeeded")]
        Succeeded = 4,
        [EnumMember(Value = "failed")]
        Failed = 5,
        [EnumMember(Value = "cancelled")]
        Cancelled = 6,
    }
}