// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// A helper to create a vector store with file_ids and attach it to an assistant/thread.
    /// There can be a maximum of 1 vector store attached to the assistant/thread.
    /// </summary>
    public sealed class VectorStoreRequest
    {
        public VectorStoreRequest() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileIds">
        /// A list of file IDs to add to the vector store.
        /// There can be a maximum of 10000 files in a vector store.
        /// </param>
        /// <param name="metadata">
        /// Optional, set of 16 key-value pairs that can be attached to a vector store.
        /// This can be useful for storing additional information about the vector store in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        public VectorStoreRequest(IReadOnlyList<string> fileIds, IReadOnlyDictionary<string, string> metadata = null)
        {
            FileIds = fileIds;
            Metadata = metadata;
        }

        /// <inheritdoc />
        public VectorStoreRequest(string fileId, IReadOnlyDictionary<string, string> metadata = null)
            : this(new List<string> { fileId }, metadata)
        {
        }

        /// <summary>
        /// A list of file IDs to add to the vector store.
        /// There can be a maximum of 10000 files in a vector store.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_ids")]
        public IReadOnlyList<string> FileIds { get; private set; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to a vector store.
        /// This can be useful for storing additional information about the vector store in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        public static implicit operator VectorStoreRequest(string fileId) => new(fileId);

        public static implicit operator VectorStoreRequest(List<string> fileIds) => new(fileIds);
    }
}
