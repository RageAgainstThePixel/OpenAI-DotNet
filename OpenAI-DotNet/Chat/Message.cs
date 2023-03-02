using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Message
    {
        [JsonConstructor]
        public Message(
            string role,
            string content)
        {
            Role = role;
            Content = content;
        }

        [JsonPropertyName("role")]
        public string Role { get; }

        [JsonPropertyName("content")]
        public string Content { get; }

        public override string ToString() => Content;

        public static implicit operator string(Message message) => message.Content;
    }
}
