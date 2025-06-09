// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class MCPToolList
    {
        [JsonConstructor]
        public MCPToolList(IEnumerable<string> toolNames)
        {
            ToolNames = toolNames?.ToList() ?? throw new ArgumentNullException(nameof(toolNames));
        }

        [JsonPropertyName("tool_names")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string> ToolNames { get; }
    }
}
