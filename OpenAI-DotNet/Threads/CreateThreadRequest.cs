// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class CreateThreadRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="messages">
        /// A list of messages to start the thread with.
        /// </param>
        /// <param name="toolResources">
        /// A set of resources that are made available to the assistant's tools in this thread.
        /// The resources are specific to the type of tool.
        /// For example, the code_interpreter tool requires a list of file IDs,
        /// while the file_search tool requires a list of vector store IDs.
        /// </param>
        /// <param name="metadata">
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        public CreateThreadRequest(IEnumerable<Message> messages = null, ToolResources toolResources = null, IReadOnlyDictionary<string, string> metadata = null)
        {
            Messages = messages?.ToList();
            ToolResources = toolResources;
            Metadata = metadata;
        }

        /// <inheritdoc />
        public CreateThreadRequest(string message) : this(new[] { new Message(message) })
        {
        }

        /// <summary>
        /// A list of messages to start the thread with.
        /// </summary>
        [JsonPropertyName("messages")]
        public IReadOnlyList<Message> Messages { get; }

        /// <summary>
        /// A set of resources that are made available to the assistant's tools in this thread.
        /// The resources are specific to the type of tool.
        /// For example, the code_interpreter tool requires a list of file IDs,
        /// while the file_search tool requires a list of vector store IDs.
        /// </summary>
        [JsonPropertyName("tool_resources")]
        public ToolResources ToolResources { get; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; }

        public static implicit operator CreateThreadRequest(string message) => new(message);
    }
}
