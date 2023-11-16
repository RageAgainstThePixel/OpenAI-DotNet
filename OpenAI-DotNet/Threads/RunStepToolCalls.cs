using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class RunStepToolCalls
    {
        /// <summary>
        /// An array of tool calls the run step was involved in.
        /// These can be associated with one of three types of tools: 'code_interpreter', 'retrieval', or 'function'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tool_calls")]
        public IReadOnlyList<Tool> ToolCalls { get; private set; }
    }
}