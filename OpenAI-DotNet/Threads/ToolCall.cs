using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class ToolCall
    {
        /// <summary>
        /// The ID of the tool call.
        /// This ID must be referenced when you submit the tool outputs in using the Submit tool outputs to run endpoint.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The type of tool call the output is required for. For now, this is always 'function'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The function definition.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("function")]
        public FunctionCall FunctionCall { get; private set; }
    }
}