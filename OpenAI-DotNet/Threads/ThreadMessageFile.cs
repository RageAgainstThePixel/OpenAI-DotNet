using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class ThreadMessageFile
    {
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always thread.message.file.
        /// </summary>
        /// <returns></returns>
        [JsonPropertyName("object")]
        public string Object { get; private set;  }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the message file was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public int CreatedAt { get; private set;  }

        /// <summary>
        /// The ID of the message that the File is attached to.
        /// </summary>
        [JsonPropertyName("message_id")]
        public string MessageId { get; private set;  }
    }
}