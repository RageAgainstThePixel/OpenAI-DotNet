// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    /// <summary>
    /// Tool function call output
    /// </summary>
    public sealed class ToolOutput
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="toolCallId">
        /// The ID of the tool call in the <see cref="RequiredAction"/> within the <see cref="RunResponse"/> the output is being submitted for.
        /// </param>
        /// <param name="output">
        /// The output of the tool call to be submitted to continue the run.
        /// </param>
        [JsonConstructor]
        public ToolOutput(string toolCallId, string output)
        {
            ToolCallId = toolCallId;
            Output = output;
        }

        /// <summary>
        /// The ID of the tool call in the <see cref="RequiredAction"/> within the <see cref="RunResponse"/> the output is being submitted for.
        /// </summary>
        [JsonPropertyName("tool_call_id")]
        public string ToolCallId { get; }

        /// <summary>
        /// The output of the tool call to be submitted to continue the run.
        /// </summary>
        [JsonPropertyName("output")]
        public string Output { get; }
    }
}