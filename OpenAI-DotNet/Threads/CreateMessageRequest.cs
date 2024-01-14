// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class CreateMessageRequest
    {
        public static implicit operator CreateMessageRequest(string content) => new(content);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fileIds"></param>
        /// <param name="metadata"></param>
        public CreateMessageRequest(string content, IEnumerable<string> fileIds = null, IReadOnlyDictionary<string, string> metadata = null)
        {
            Role = Role.User;
            Content = content;
            FileIds = fileIds?.ToList();
            Metadata = metadata;
        }

        /// <summary>
        /// The role of the entity that is creating the message.
        /// </summary>
        /// <remarks>
        /// Currently only user is supported.
        /// </remarks>
        [JsonPropertyName("role")]
        public Role Role { get; }

        /// <summary>
        /// The content of the message.
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; }

        /// <summary>
        /// A list of File IDs that the message should use. There can be a maximum of 10 files attached to a message.
        /// Useful for tools like retrieval and code_interpreter that can access and use files.
        /// </summary>
        [JsonPropertyName("file_ids")]
        public IReadOnlyList<string> FileIds { get; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; }
    }
}