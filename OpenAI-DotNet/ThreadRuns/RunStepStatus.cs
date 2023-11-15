using System.Runtime.Serialization;

namespace OpenAI.ThreadRuns;

public enum RunStepStatus
{
    [EnumMember(Value = "in_progress")]
    InProgress,
    [EnumMember(Value = "cancelled")]
    Cancelled,
    [EnumMember(Value = "failed")]
    Failed,
    [EnumMember(Value = "completed")]
    Completed,
    [EnumMember(Value = "expired")]
    Expired
}