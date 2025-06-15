// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Responses
{
    public enum ResponseItemType
    {
        [EnumMember(Value = "message")]
        Message,
        [EnumMember(Value = "computer_call")]
        ComputerCall,
        [EnumMember(Value = "computer_call_output")]
        ComputerCallOutput,
        [EnumMember(Value = "function_call")]
        FunctionCall,
        [EnumMember(Value = "function_call_output")]
        FunctionCallOutput,
        [EnumMember(Value = "image_generation_call")]
        ImageGenerationCall,
        [EnumMember(Value = "local_shell_call")]
        LocalShellCall,
        [EnumMember(Value = "local_shell_call_output")]
        LocalShellCallOutput,
        [EnumMember(Value = "file_search_call")]
        FileSearchCall,
        [EnumMember(Value = "web_search_call")]
        WebSearchCall,
        [EnumMember(Value = "reasoning")]
        Reasoning,
        [EnumMember(Value = "mcp_call")]
        McpCall,
        [EnumMember(Value = "mcp_approval_request")]
        McpApprovalRequest,
        [EnumMember(Value = "mcp_approval_response")]
        McpApprovalResponse,
        [EnumMember(Value = "mcp_list_tools")]
        McpListTools,
        [EnumMember(Value = "item_reference")]
        ItemReference,
    }
}
