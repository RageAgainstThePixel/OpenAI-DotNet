using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads;

public class CreateThreadRequest
{
    /// <summary>
    /// A list of messages to start the thread with.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; } = new();

    /// <summary>
    /// Set of 16 key-value pairs that can be attached to an object.
    /// This can be useful for storing additional information about the object in a structured format.
    /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object> Metadata { get; set; }

    public class Message
    {
        public Message() { }

        public Message(string content)
        {
            Content = content;
        }

        /// <summary>
        /// The role of the entity that is creating the message. Currently only user is supported.
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = "user";

        /// <summary>
        /// The content of the message.
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        /// <summary>
        /// A list of File IDs that the message should use. There can be a maximum of 10 files attached to a message.
        /// Useful for tools like retrieval and code_interpreter that can access and use files.
        /// </summary>
        [JsonPropertyName("file_ids")]
        public string[] FileIds { get; set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; set; }
    }
}