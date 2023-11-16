using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    /// <summary>
    /// Request to submit tool outputs to run
    /// </summary>
    public sealed class SubmitThreadRunToolOutputsRequest
    {
        public SubmitThreadRunToolOutputsRequest(IReadOnlyList<ToolOutput> toolOutputs)
        {
            ToolOutputs = toolOutputs;
        }

        /// <summary>
        /// A list of tools for which the outputs are being submitted.
        /// </summary>
        [JsonPropertyName("tool_outputs")]
        public IReadOnlyList<ToolOutput> ToolOutputs { get; }
    }
}