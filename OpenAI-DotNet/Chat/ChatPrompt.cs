using System;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    [Obsolete("Use OpenAI.Chat.Message instead")]
    public sealed class ChatPrompt
    {
        [Obsolete("Use OpenAI.Chat.Message instead")]
        public ChatPrompt(string role, string content)
        {
            Role = role.ToLower() switch
            {
                "system" => Role.System,
                "assistant" => Role.Assistant,
                "user" => Role.User,
                _ => throw new ArgumentException(nameof(role))
            };
            Content = content;
        }

        [JsonConstructor]
        [Obsolete("Use OpenAI.Chat.Message instead")]
        public ChatPrompt(Role role, string content)
        {
            Role = role;
            Content = content;
        }

        [JsonPropertyName("role")]
        public Role Role { get; }

        [JsonPropertyName("content")]
        public string Content { get; }

        public static implicit operator ChatPrompt(Message message) => new ChatPrompt(message.Role, message.Content);

        public static implicit operator Message(ChatPrompt message) => new Message(message.Role, message.Content);
    }
}
