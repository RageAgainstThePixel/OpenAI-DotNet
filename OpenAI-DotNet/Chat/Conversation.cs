using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Conversation
    {
        [JsonConstructor]
        public Conversation(List<Message> messages)
        {
            this.messages = messages;
        }

        private readonly List<Message> messages;

        [JsonPropertyName("messages")]
        public IReadOnlyList<Message> Messages => messages;

        /// <summary>
        /// Appends <see cref="Message"/> to the end of <see cref="Messages"/>.
        /// </summary>
        /// <param name="message">The message to add to the <see cref="Conversation"/>.</param>
        public void AppendMessage(Message message) => messages.Add(message);

        public override string ToString() => JsonSerializer.Serialize(this, OpenAIClient.JsonSerializationOptions);

        public static implicit operator string(Conversation conversation) => conversation.ToString();
    }
}