using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class SubmitToolOutputs
    {
        /// <summary>
        /// A list of the relevant tool calls.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tool_calls")]
        public IReadOnlyList<ToolCall> ToolCalls { get; private set; }
    }
}