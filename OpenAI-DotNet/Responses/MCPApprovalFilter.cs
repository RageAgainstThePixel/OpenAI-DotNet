// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class MCPApprovalFilter
    {
        [JsonConstructor]
        public MCPApprovalFilter(MCPToolList always = null, MCPToolList never = null)
        {
            Always = always;
            Never = never;
        }

        [JsonPropertyName("always")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MCPToolList Always { get; }

        [JsonPropertyName("never")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MCPToolList Never { get; }
    }
}
