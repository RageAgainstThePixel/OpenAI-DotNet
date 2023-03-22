using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class ChatPrompt
    {
        [JsonConstructor]
        public ChatPrompt(string role, string content)
        {
            this.Role = role;
            this.Content = content;
        }

        [JsonPropertyName("role")]
        public string Role { get; }

        [JsonPropertyName("content")]
        public string Content { get; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
