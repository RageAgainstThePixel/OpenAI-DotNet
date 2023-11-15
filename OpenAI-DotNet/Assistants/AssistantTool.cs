using System.Text.Json.Serialization;
using OpenAI.Chat;
using OpenAI.Extensions;

namespace OpenAI.Assistants
{
    public sealed class AssistantTool
    {
        public AssistantTool() { }

        public AssistantTool(AssistantToolType type)
        {
            Type = type;
        }

        public AssistantTool(Function function)
        {
            Type = AssistantToolType.Function;
            Function = function;
        }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter<AssistantToolType>))]
        public AssistantToolType Type { get; set; }
    
        [JsonPropertyName("function")]
        public Function Function { get; set; }
    }
}