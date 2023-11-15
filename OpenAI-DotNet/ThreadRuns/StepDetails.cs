using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.ThreadRuns;

public sealed class StepDetails
{
    /// <summary>
    /// Type of step details
    /// </summary>
    [JsonPropertyName("type")]
    public StepDetailsType Type { get; set; }

    /// <summary>
    /// Details of the message creation by the run step.
    /// </summary>
    [JsonPropertyName("message_creation")]
    public RunStepMessageCreation MessageCreation { get; set; }

    /// <summary>
    /// An array of tool calls the run step was involved in.
    /// These can be associated with one of three types of tools: code_interpreter, retrieval, or function.
    /// </summary>
    [JsonPropertyName("tool_calls")]
    public IReadOnlyList<RunStepToolCall> ToolCalls { get; set; }
}