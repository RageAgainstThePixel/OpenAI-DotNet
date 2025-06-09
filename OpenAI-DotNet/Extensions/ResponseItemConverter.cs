// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    internal class ResponseItemConverter : JsonConverter<IResponseItem>
    {
        public override void Write(Utf8JsonWriter writer, IResponseItem value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), options);

        public override IResponseItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var root = JsonDocument.ParseValue(ref reader).RootElement;
            var type = root.GetProperty("type").GetString()!;

            return type switch
            {
                "message" => root.Deserialize<Message>(options),
                "computer_call" => root.Deserialize<ComputerToolCall>(options),
                "computer_call_output" => root.Deserialize<ComputerToolCall>(options),
                "function_call" => root.Deserialize<FunctionToolCall>(options),
                "function_call_output" => root.Deserialize<FunctionToolCallOutput>(options),
                "image_generation_call" => root.Deserialize<ImageGenerationCall>(options),
                "local_shell_call" => root.Deserialize<LocalShellCall>(options),
                "local_shell_call_output" => root.Deserialize<LocalShellCall>(options),
                "file_search_call" => root.Deserialize<FileSearchToolCall>(options),
                "web_search_call" => root.Deserialize<WebSearchToolCall>(options),
                "reasoning" => root.Deserialize<ReasoningItem>(options),
                "mcp_call" => root.Deserialize<MCPToolCall>(options),
                "mcp_approval_request" => root.Deserialize<MCPApprovalRequest>(options),
                "mcp_approval_response" => root.Deserialize<MCPApprovalResponse>(options),
                "mcp_list_tools" => root.Deserialize<MCPListTools>(options),
                "item_reference" => root.Deserialize<ItemReference>(options),
                _ => throw new NotImplementedException($"Unknown response item type: {type}")
            };
        }
    }
}
