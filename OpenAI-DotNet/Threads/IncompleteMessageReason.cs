using System.Runtime.Serialization;

namespace OpenAI.Threads
{
    public enum IncompleteMessageReason
    {
        None = 0,
        [EnumMember(Value = "content_filter")]
        ContentFilter,
        [EnumMember(Value = "max_tokens")]
        MaxTokens,
        [EnumMember(Value = "max_completion_tokens")]
        MaxCompletionTokens,
        [EnumMember(Value = "max_prompt_tokens")]
        MaxPromptTokens,
        [EnumMember(Value = "run_cancelled")]
        RunCancelled,
        [EnumMember(Value = "run_expired")]
        RunExpired,
        [EnumMember(Value = "run_failed")]
        RunFailed
    }
}
