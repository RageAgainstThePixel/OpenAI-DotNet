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

    public sealed class RunStepToolCall
    {
        /// <summary>
        /// The ID of the tool call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The type of tool call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The Code Interpreter tool call definition.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code_interpreter")]
        public CodeInterpreter CodeInterpreter { get; private set; }
    }
}