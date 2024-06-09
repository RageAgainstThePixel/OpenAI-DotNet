// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.VectorStores
{
    public sealed class CreateVectorStoreRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileIds">
        /// A list of file IDs to add to the vector store.
        /// There can be a maximum of 10000 files in a vector store.
        /// </param>
        /// <param name="name"></param>
        /// <param name="expiresAfter"></param>
        /// <param name="chunkingStrategy">
        /// The chunking strategy used to chunk the file(s). If not set, will use the auto strategy. Only applicable if file_ids is non-empty.
        /// </param>
        /// <param name="metadata">
        /// Optional, set of 16 key-value pairs that can be attached to a vector store.
        /// This can be useful for storing additional information about the vector store in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        public CreateVectorStoreRequest(IReadOnlyList<string> fileIds = null, string name = null, int? expiresAfter = null, ChunkingStrategy chunkingStrategy = null, IReadOnlyDictionary<string, string> metadata = null)
        {
            FileIds = fileIds;
            Name = name;
            ExpiresAfter = expiresAfter.HasValue ? new ExpirationPolicy(expiresAfter.Value) : null;
            ChunkingStrategy = chunkingStrategy ?? new ChunkingStrategy(ChunkingStrategyType.Auto);
            Metadata = metadata;
        }

        /// <inheritdoc />
        public CreateVectorStoreRequest(string fileId, string name = null, int? expiresAfter = null, ChunkingStrategy chunkingStrategy = null, IReadOnlyDictionary<string, string> metadata = null)
            : this(new List<string> { fileId }, name, expiresAfter, chunkingStrategy, metadata)
        {
        }

        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; }

        /// <summary>
        /// A list of file IDs to add to the vector store.
        /// There can be a maximum of 10000 files in a vector store.
        /// </summary>
        [JsonPropertyName("file_ids")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyList<string> FileIds { get; }

        /// <summary>
        /// The expiration policy for a vector store.
        /// </summary>
        [JsonPropertyName("expires_after")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ExpirationPolicy ExpiresAfter { get; }

        [JsonPropertyName("chunking_strategy")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ChunkingStrategy ChunkingStrategy { get; }

        /// <summary>
        /// Set of 16 key-value pairs that can be attached to a vector store.
        /// This can be useful for storing additional information about the vector store in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </summary>
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IReadOnlyDictionary<string, string> Metadata { get; }

        public static implicit operator CreateVectorStoreRequest(string fileId) => new(fileId);

        public static implicit operator CreateVectorStoreRequest(List<string> fileIds) => new(fileIds);
    }
}
