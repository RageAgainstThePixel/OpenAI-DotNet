// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.VectorStores
{
    /// <summary>
    /// A batch of files attached to a vector store.
    /// </summary>
    public sealed class VectorStoreFileBatchResponse : BaseResponse
    {
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always `vector_store.file_batch`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the vector store files batch was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        /// <summary>
        /// The ID of the vector store that the files is attached to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("vector_store_id")]
        public string VectorStoreId { get; private set; }

        /// <summary>
        /// The status of the vector store files batch, which can be either `in_progress`, `completed`, `cancelled` or `failed`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public VectorStoreFileStatus Status { get; private set; }

        [JsonInclude]
        [JsonPropertyName("file_counts")]
        public FileCounts FileCounts { get; private set; }

        public override string ToString() => Id;

        public static implicit operator string(VectorStoreFileBatchResponse response) => response?.ToString();
    }
}
