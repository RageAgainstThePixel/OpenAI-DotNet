using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Message
    {
        public Message(Role role, string content)
        {
            Role = role;
            Content = content;
        }

        [JsonInclude]
        [JsonPropertyName("role")]
        public Role Role { get; private set; }

        [JsonInclude]
        [JsonPropertyName("content")]
        public string Content { get; private set; }

        public static implicit operator string(Message message) => message.Content;
    }
}
