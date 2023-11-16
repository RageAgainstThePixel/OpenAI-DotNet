using System.Text.Json.Serialization;
using OpenAI.Chat;

namespace OpenAI.Assistants
{
    public sealed class AssistantTool
    {
        public AssistantTool() { }

        public AssistantTool(string type)
        {
            Type = type;
        }

        public static AssistantTool CodeInterpreter => new ("code_interpreter");
        public static AssistantTool Retrieval => new ("retrieval");
        public static AssistantTool ForFunction(Function function) => new("function") { Function = function };

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("function")]
        public Function Function { get; set; }
    }
}