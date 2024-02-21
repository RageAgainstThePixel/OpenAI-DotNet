// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class SubmitToolOutputsRequest
    {
        /// <summary>
        /// Tool output to be submitted.
        /// </summary>
        /// <param name="toolOutput"><see cref="ToolOutput"/>.</param>
        public SubmitToolOutputsRequest(ToolOutput toolOutput)
            : this(new[] { toolOutput })
        {
        }

        /// <summary>
        /// A list of tools for which the outputs are being submitted.
        /// </summary>
        /// <param name="toolOutputs">Collection of tools for which the outputs are being submitted.</param>
        public SubmitToolOutputsRequest(IEnumerable<ToolOutput> toolOutputs)
            => ToolOutputs = toolOutputs?.ToList();

        /// <summary>
        /// A list of tools for which the outputs are being submitted.
        /// </summary>
        [JsonPropertyName("tool_outputs")]
        public IReadOnlyList<ToolOutput> ToolOutputs { get; }

        public static implicit operator SubmitToolOutputsRequest(ToolOutput toolOutput) => new(toolOutput);

        public static implicit operator SubmitToolOutputsRequest(ToolOutput[] toolOutputs) => new(toolOutputs);

        public static implicit operator SubmitToolOutputsRequest(List<ToolOutput> toolOutputs) => new(toolOutputs);
    }
}