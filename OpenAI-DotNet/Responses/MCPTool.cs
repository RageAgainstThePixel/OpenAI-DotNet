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

        [JsonPropertyName("type")]
        public string Type => "mcp";

        [JsonInclude]
        [JsonPropertyName("server_label")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ServerLabel { get; private set; }

        [JsonInclude]
        [JsonPropertyName("server_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ServerUrl { get; private set; }

        [JsonInclude]
        [JsonPropertyName("allowed_tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string> AllowedTools { get; private set; }

        [JsonInclude]
        [JsonPropertyName("headers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyDictionary<string, object> Headers { get; private set; }

        /// <summary>
        /// Specify which of the MCP server's tools require approval.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("require_approval")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonConverter(typeof(StringOrObjectConverter<MCPApprovalFilter>))]
        public object RequireApproval { get; }
    }
}
