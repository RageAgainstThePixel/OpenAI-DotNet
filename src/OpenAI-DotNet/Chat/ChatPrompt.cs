using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class ChatPrompt
    {
        [JsonConstructor]
        public ChatPrompt(string role, string content)
        {
            Role = role;
            Content = content;
        }

        [JsonPropertyName("role")]
        public string Role { get; }

        [JsonPropertyName("content")]
        public string Content { get; }

        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
