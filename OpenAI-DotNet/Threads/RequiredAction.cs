using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class RequiredAction
    {
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// Details on the tool outputs needed for this run to continue.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("submit_tool_outputs")]
        public SubmitToolOutputs SubmitToolOutputs { get; private set; }
    }
}