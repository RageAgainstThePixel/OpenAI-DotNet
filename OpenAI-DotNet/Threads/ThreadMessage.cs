using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class ThreadMessage
    {
        public static implicit operator string(ThreadMessage message) => message?.Id;

        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always thread.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the thread was created.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The thread ID that this message belongs to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("thread_id")]
        public string ThreadId { get; private set; }

        /// <summary>
        /// The entity that produced the message. One of user or assistant.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("role")]
        public ThreadRole Role { get; private set; }

        /// <summary>
        /// The content of the message in array of text and/or images.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("content")]
        public ThreadMessageContent[] Content { get; private set; }

        /// <summary>
        /// If applicable, the ID of the assistant that authored this message.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("assistant_id")]
        public string AssistantId { get; private set; }

        /// <summary>
        /// If applicable, the ID of the run associated with the authoring of this message.
        /// </summary>
        /// <returns></returns>
        [JsonInclude]
        [JsonPropertyName("run_id")]
        public string RunId { get; private set; }

        /// <summary>
        /// A list of file IDs that the assistant should use. Useful for tools like retrieval and code_interpreter that can access files. A maximum of 10 files can be attached to a message.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_ids")]
        public string[] FileIds { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; private set; }
    }
}