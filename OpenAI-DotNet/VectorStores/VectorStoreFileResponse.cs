// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.VectorStores
{
    /// <summary>
    /// A list of files attached to a vector store.
    /// </summary>
    public sealed class VectorStoreFileResponse : BaseResponse
    {
        /// <summary>
        /// The identifier, which can be referenced in API endpoints.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <summary>
        /// The object type, which is always 'vector_store.file'.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <summary>
        /// The total vector store usage in bytes. Note that this may be different from the original file size.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("usage_bytes")]
        public long UsageBytes { get; private set; }

        /// <summary>
        /// The Unix timestamp (in seconds) for when the vector store file was created.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTimeOffset CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds);

        /// <summary>
        /// The ID of the vector store that the file is attached to.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("vector_store_id")]
        public string VectorStoreId { get; private set; }

        /// <summary>
        /// The status of the vector store file, which can be either 'in_progress', 'completed', 'cancelled', or 'failed'.
        /// The status 'completed' indicates that the vector store file is ready for use.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("status")]
        public VectorStoreFileStatus Status { get; private set; }

        /// <summary>
        /// The last error associated with this vector store file. Will be 'null' if there are no errors.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("last_error")]
        public Error LastError { get; private set; }

        /// <summary>
        /// The strategy used to chunk the file.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("chunking_strategy")]
        public ChunkingStrategy ChunkingStrategy { get; private set; }

        public override string ToString() => Id;

        public static implicit operator string(VectorStoreFileResponse response) => response?.ToString();
    }
}
