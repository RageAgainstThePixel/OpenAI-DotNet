using System.Text.Json.Serialization;

namespace OpenAI.ThreadRuns
{
    /// <summary>
    /// Tool function call output
    /// </summary>
    public sealed class ToolOutput
    {
        public ToolOutput(string toolCallId, string output)
        {
            ToolCallId = toolCallId;
            Output = output;
        }

        /// <summary>
        /// The ID of the tool call in the required_action object within the run object the output is being submitted for.
        /// </summary>
        [JsonPropertyName("tool_call_id")]
        public string ToolCallId { get; set; }

        /// <summary>
        /// The output of the tool call to be submitted to continue the run.
        /// </summary>
        [JsonPropertyName("output")]
        public string Output { get; set; }
    }
}