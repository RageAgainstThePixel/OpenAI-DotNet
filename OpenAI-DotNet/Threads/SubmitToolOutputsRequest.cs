using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class SubmitToolOutputsRequest
    {
        public SubmitToolOutputsRequest(IEnumerable<ToolOutput> toolOutputs)
        {
            ToolOutputs = toolOutputs?.ToList();
        }

        /// <summary>
        /// A list of tools for which the outputs are being submitted.
        /// </summary>
        [JsonPropertyName("tool_outputs")]
        public IReadOnlyList<ToolOutput> ToolOutputs { get; }
    }
}