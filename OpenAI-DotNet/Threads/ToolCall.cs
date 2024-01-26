// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        /// The type of tool call the output is required for.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The definition of the function that was called.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("function")]
        public FunctionCall FunctionCall { get; private set; }

        /// <summary>
        /// The Code Interpreter tool call definition.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("code_interpreter")]
        public CodeInterpreter CodeInterpreter { get; private set; }

        /// <summary>
        /// For now, this is always going to be an empty object.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("retrieval")]
        public object Retrieval { get; private set; }
    }
}