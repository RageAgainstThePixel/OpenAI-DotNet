// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    internal class ToolConverter : JsonConverter<ITool>
    {
        public override void Write(Utf8JsonWriter writer, ITool value, JsonSerializerOptions options)
            => JsonSerializer.Serialize(writer, value, value.GetType(), options);

        public override ITool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var root = JsonDocument.ParseValue(ref reader).RootElement;
            var type = root.GetProperty("type").GetString()!;

            return type switch
            {
                "code_interpreter" => root.Deserialize<CodeInterpreterTool>(options),
                "computer_user_preview" => root.Deserialize<ComputerUsePreviewTool>(options),
                "file_search" => root.Deserialize<FileSearchTool>(options),
                "function" => root.Deserialize<Function>(options),
                "image_generation" => root.Deserialize<ImageGenerationTool>(options),
                "local_shell" => root.Deserialize<LocalShellTool>(options),
                "mcp" => root.Deserialize<MCPTool>(options),
                "tool" => root.Deserialize<Tool>(options),
                "web_search_preview" => root.Deserialize<WebSearchPreviewTool>(options),
                _ => throw new NotImplementedException($"Unknown tool item type: {type}")
            };
        }
    }
}
