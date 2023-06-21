using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Message
    {
        internal Message(Delta other)
        {
            CopyFrom(other);
        }

        /// <summary>
        /// Creates a new message to insert into a chat conversation.
        /// </summary>
        /// <param name="role">
        /// The <see cref="Chat.Role"/> of the author of this message.
        /// </param>
        /// <param name="content">
        /// The contents of the message.
        /// </param>
        /// <param name="name">
        /// Optional, The name of the author of this message.<br/>
        /// May contain a-z, A-Z, 0-9, and underscores, with a maximum length of 64 characters.
        /// </param>
        /// <param name="function">
        /// The function that should be called, as generated by the model.
        /// </param>
        public Message(Role role, string content, string name = null, Function function = null)
        {
            Role = role;
            Content = content;
            Name = name;
            Function = function;
        }

        /// <summary>
        /// The <see cref="Chat.Role"/> of the author of this message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("role")]
        public Role Role { get; private set; }

        /// <summary>
        /// The contents of the message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Content { get; private set; }

        /// <summary>
        /// The function that should be called, as generated by the model.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("function_call")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Function Function { get; private set; }

        /// <summary>
        /// Optional, The name of the author of this message.<br/>
        /// May contain a-z, A-Z, 0-9, and underscores, with a maximum length of 64 characters.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; private set; }

        public override string ToString() => Content ?? string.Empty;

        public static implicit operator string(Message message) => message.ToString();

        internal void CopyFrom(Delta other)
        {
            if (Role == 0 &&
                other?.Role > 0)
            {
                Role = other.Role;
            }

            if (!string.IsNullOrEmpty(other?.Content))
            {
                Content += other.Content;
            }

            if (!string.IsNullOrWhiteSpace(other?.Name))
            {
                Name = other.Name;
            }

            if (other?.Function != null)
            {
                if (Function == null)
                {
                    Function = new Function(other);
                }
                else
                {
                    Function.CopyFrom(other);
                }
            }
        }
    }
}
