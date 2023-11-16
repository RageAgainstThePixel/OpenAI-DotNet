using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
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