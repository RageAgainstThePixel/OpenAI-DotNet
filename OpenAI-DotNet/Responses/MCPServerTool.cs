// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class MCPServerTool
    {
        [JsonInclude]
        [JsonPropertyName("name")]
        public string Name { get; private set; }

        [JsonInclude]
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; private set; }

        [JsonInclude]
        [JsonPropertyName("input_schema")]
        public JsonNode InputSchema { get; private set; }

        [JsonInclude]
        [JsonPropertyName("annotations")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonNode Annotations { get; private set; }
    }
}
