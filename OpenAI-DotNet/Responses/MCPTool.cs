// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    /// <summary>
    /// Give the model access to additional tools via remote Model Context Protocol (MCP) servers.
    /// <see href="https://platform.openai.com/docs/guides/tools-remote-mcp"/>
    /// </summary>
    public sealed class MCPTool : ITool
    {
        public static implicit operator Tool(MCPTool mcpTool) => new(mcpTool as ITool);

        public MCPTool() { }

        public MCPTool(
            string serverLabel,
            string serverUrl,
            string connectorId,
            string authorization,
            string serverDescription,
            IReadOnlyList<string> allowedTools,
            IReadOnlyDictionary<string, object> headers,
            MCPToolRequireApproval requireApproval)
            : this(
                serverLabel: serverLabel,
                serverUrl: serverUrl,
                connectorId: connectorId,
                authorization: authorization,
                serverDescription: serverDescription,
                allowedTools: allowedTools,
                headers: headers,
                requireApproval: (object)requireApproval)
        {
        }

        public MCPTool(
            string serverLabel,
            string serverUrl,
            string connectorId,
            string authorization,
            string serverDescription,
            IReadOnlyList<string> allowedTools,
            IReadOnlyDictionary<string, object> headers,
            MCPApprovalFilter requireApproval)
            : this(
                serverLabel: serverLabel,
                serverUrl: serverUrl,
                connectorId: connectorId,
                authorization: authorization,
                serverDescription: serverDescription,
                allowedTools: allowedTools,
                headers: headers,
                requireApproval: (object)requireApproval)
        {
        }

        public MCPTool(
            string serverLabel,
            string serverUrl = null,
            string connectorId = null,
            string authorization = null,
            string serverDescription = null,
            IReadOnlyList<object> allowedTools = null,
            IReadOnlyDictionary<string, object> headers = null,
            object requireApproval = null)
        {
            ServerLabel = serverLabel;
            ServerUrl = serverUrl;
            ConnectorId = connectorId;
            Authorization = authorization;
            ServerDescription = serverDescription;
            AllowedTools = allowedTools;
            Headers = headers ?? new Dictionary<string, object>();
            RequireApproval = requireApproval is MCPToolRequireApproval approval ? approval.ToString().ToLower() : requireApproval;
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ServerUrl { get; private set; }

        /// <summary>
        /// Identifier for service connectors, like those available in ChatGPT. One of
        /// <see cref="ServerUrl"/> or <see cref="ConnectorId"/> must be provided. Learn more about service
        /// connectors <see href="https://platform.openai.com/docs/guides/tools-remote-mcp#connectors"/>.<br/>
        /// Currently supported `connector_id` values are:<br/>
        /// <list type="bullet">
        /// <item>
        /// <description>Dropbox: <c>connector_dropbox</c></description>
        /// </item>
        /// <item>
        /// <description>Gmail: <c>connector_gmail</c></description>
        /// </item>
        /// <item>
        /// <description>Google Calendar: <c>connector_googlecalendar</c></description>
        /// </item>
        /// <item>
        /// <description>Google Drive: <c>connector_googledrive</c></description>
        /// </item>
        /// <item>
        /// <description>Microsoft Teams: <c>connector_microsoftteams</c></description>
        /// </item>
        /// <item>
        /// <description>Outlook Calendar: <c>connector_outlookcalendar</c></description>
        /// </item>
        /// <item>
        /// <description>Outlook Email: <c>connector_outlookemail</c></description>
        /// </item>
        /// <item>
        /// <description>SharePoint: <c>connector_sharepoint</c></description>
        /// </item>
        /// </list>
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("connector_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ConnectorId { get; }

        /// <summary>
        /// An OAuth access token that can be used with a remote MCP server, either
        /// with a custom MCP server URL or a service connector. Your application
        /// must handle the OAuth authorization flow and provide the token here.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("authorization")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Authorization { get; }

        /// <summary>
        /// Optional description of the MCP server, used to provide more context.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("server_description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string ServerDescription { get; }

        /// <summary>
        /// List of allowed tool names.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("allowed_tools")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<object> AllowedTools { get; private set; }

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
