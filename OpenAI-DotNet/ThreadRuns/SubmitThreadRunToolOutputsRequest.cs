using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.ThreadRuns;

/// <summary>
/// Request to submit tool outputs to run
/// </summary>
public sealed class SubmitThreadRunToolOutputsRequest
{
    /// <summary>
    /// A list of tools for which the outputs are being submitted.
    /// </summary>
    [JsonPropertyName("tool_outputs")]
    public List<ToolOutput> ToolOutputs { get; set; }
}