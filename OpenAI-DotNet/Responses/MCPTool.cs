// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// Give the model access to additional tools via remote Model Context Protocol (MCP) servers.
    /// </summary>
    public sealed class MCPTool : ITool
    {
        public static implicit operator Tool(MCPTool mcpTool) => new(mcpTool as ITool);

        public MCPTool() { }

        public MCPTool(
            string serverLabel,
            string serverUrl,
            IReadOnlyList<string> allowedTools,
            IReadOnlyDictionary<string, object> headers,
            string requireApproval)
            : this(serverLabel, serverUrl, allowedTools, headers, (object)requireApproval)
        {
        }

        public MCPTool(
            string serverLabel,
            string serverUrl,
            IReadOnlyList<string> allowedTools,
            IReadOnlyDictionary<string, object> headers,
            MCPApprovalFilter requireApproval)
            : this(serverLabel, serverUrl, allowedTools, headers, (object)requireApproval)
        {
        }

        public MCPTool(
            string serverLabel,
            string serverUrl,
            IReadOnlyList<string> allowedTools = null,
            IReadOnlyDictionary<string, object> headers = null,
            object requireApproval = null)
        {
            ServerLabel = serverLabel;
            ServerUrl = serverUrl;
            AllowedTools = allowedTools;
            Headers = headers ?? new Dictionary<string, object>();
            RequireApproval = requireApproval;
        }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; } = "mcp";

        /// <summary>
        /// A label for this MCP server, used to identify it in tool calls.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_label")]
        public string ServerLabel { get; private set; }

        /// <summary>
        /// The URL for the MCP server.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_url")]
        public string ServerUrl { get; private set; }

        /// <summary>
        /// List of allowed tool names.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("allowed_tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string> AllowedTools { get; private set; }

        /// <summary>
        /// Optional HTTP headers to send to the MCP server. Use for authentication or other purposes.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("headers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyDictionary<string, object> Headers { get; private set; }

        /// <summary>
        /// Specify which of the MCP server's tools require approval.
        /// Can be one of <see cref="MCPApprovalFilter"/>, "always", or "never".
        /// When set to "never", all tools will not require approval.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("require_approval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonConverter(typeof(StringOrObjectConverter<MCPApprovalFilter>))]
        public object RequireApproval { get; private set; }
    }
}
