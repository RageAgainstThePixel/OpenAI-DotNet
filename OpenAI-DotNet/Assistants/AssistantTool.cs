using System.Text.Json.Serialization;
using OpenAI.Chat;

namespace OpenAI.Assistants
{
    public sealed class AssistantTool
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("function")]
        public Function Function { get; set; }
    }
}