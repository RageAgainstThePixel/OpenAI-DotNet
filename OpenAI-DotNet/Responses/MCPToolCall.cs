// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// An invocation of a tool on an MCP server.
    /// </summary>
    public sealed class MCPToolCall : BaseResponse, IResponseItem
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseItemType Type { get; private set; } = ResponseItemType.McpCall;

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("status")]
        public ResponseStatus Status { get; private set; }

        /// <summary>
        /// The name of the tool that was run.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        /// <summary>
        /// The label of the MCP server running the tool.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_label")]
        public string ServerLabel { get; private set; }

        private string argumentsString;

        private JsonNode arguments;

        /// <summary>
        /// A JSON string of the arguments to pass to the function.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("arguments")]
        public JsonNode Arguments
        {
            get
            {
                if (arguments == null)
                {
                    if (!string.IsNullOrWhiteSpace(argumentsString))
                    {
                        arguments = JsonValue.Create(argumentsString);
                    }
                    else
                    {
                        arguments = null;
                    }
                }

                return arguments;
            }
            internal set => arguments = value;
        }

        [JsonIgnore]
        internal string Delta
        {
            set
            {
                if (value == null)
                {
                    argumentsString = null;
                }
                else
                {
                    argumentsString += value;
                }
            }
        }

        /// <summary>
        /// The output from the tool call.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("output")]
        public string Output { get; private set; }

        /// <summary>
        /// The error from the tool call, if any.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("error")]
        public JsonNode Error { get; private set; }
    }
}
