using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    /// <summary>
    /// The details of the run step.
    /// </summary>
    public sealed class StepDetails
    {
        /// <summary>
        /// Details of the message creation by the run step.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("message_creation")]
        public RunStepMessageCreation MessageCreation { get; private set; }

        /// <summary>
        /// Details of the tool call.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("tool_calls")]
        public RunStepToolCalls ToolCalls { get; private set; }
    }
}