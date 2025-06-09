// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ToolCall : IToolCall, IAppendable<ToolCall>
    {
        public ToolCall() { }

        public ToolCall(string toolCallId, string functionName, JsonNode functionArguments = null)
        {
            Id = toolCallId;
            Function = new Function(functionName, arguments: functionArguments);
            Type = "function";
        }

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("index")]
        public int? Index { get; private set; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; private set; }

        [JsonInclude]
        [JsonPropertyName("function")]
        public Function Function { get; private set; }

        [JsonIgnore]
        public bool IsFunction => Type == "function";

        [JsonIgnore]
        public string CallId => Id;

        [JsonIgnore]
        public string Name => Function.Name;

        [JsonIgnore]
        public JsonNode Arguments => Function.Arguments;

        public void AppendFrom(ToolCall other)
        {
            if (other == null) { return; }

            if (!string.IsNullOrWhiteSpace(other.Id))
            {
                Id = other.Id;
            }

            if (other.Index.HasValue)
            {
                Index = other.Index.Value;
            }

            if (!string.IsNullOrWhiteSpace(other.Type))
            {
                Type = other.Type;
            }

            if (other.Function != null)
            {
                if (Function == null)
                {
                    Function = new Function(other.Function);
                }
                else
                {
                    Function.AppendFrom(other.Function);
                }
            }
        }
    }
}
