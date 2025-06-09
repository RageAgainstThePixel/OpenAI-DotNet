// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        public string Description { get; private set; }

        [JsonInclude]
        [JsonPropertyName("input_schema")]
        public string InputSchema { get; private set; }

        [JsonInclude]
        [JsonPropertyName("annotations")]
        public object Annotations { get; private set; }
    }
}
