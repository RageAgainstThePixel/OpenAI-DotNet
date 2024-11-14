// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Conversation
    {
        [JsonConstructor]
        public Conversation(List<Message> messages)
        {
            this.messages = new ConcurrentQueue<Message>();

            if (messages != null)
            {
                foreach (var message in messages)
                {
                    this.messages.Enqueue(message);
                }
            }
        }

        private readonly ConcurrentQueue<Message> messages;

        [JsonPropertyName("messages")]
        public IReadOnlyList<Message> Messages => messages.ToList();

        /// <summary>
        /// Appends <see cref="Message"/> to the end of <see cref="Messages"/>.
        /// </summary>
        /// <param name="message">The message to add to the <see cref="Conversation"/>.</param>
        public void AppendMessage(Message message) => messages.Enqueue(message);

        public override string ToString() => JsonSerializer.Serialize(this, OpenAIClient.JsonSerializationOptions);

        public static implicit operator string(Conversation conversation) => conversation?.ToString();
    }
}
