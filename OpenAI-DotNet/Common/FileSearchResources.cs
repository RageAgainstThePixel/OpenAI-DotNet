// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// <see cref="Tool.FileSearch"/> resources.
    /// </summary>
    public sealed class FileSearchResources
    {
        public FileSearchResources() { }

        /// <summary>
        /// The vector store attached to this assistant/thread.
        /// There can be a maximum of 1 vector store attached to the assistant/thread.
        /// </summary>
        /// <param name="vectorStoreId"></param>
        public FileSearchResources(string vectorStoreId = null)
        {
            VectorStoreIds = new List<string> { vectorStoreId };
        }

        /// <summary>
        /// A helper to create a vector store with file_ids and attach it to an assistant/thread.
        /// There can be a maximum of 1 vector store attached to the assistant/thread.
        /// </summary>
        /// <param name="vectorStore"><see cref="VectorStoreRequest"/>.</param>
        public FileSearchResources(VectorStoreRequest vectorStore = null)
        {
            VectorStores = new List<VectorStoreRequest> { vectorStore };
        }

        [JsonInclude]
        [JsonPropertyName("vector_store_ids")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<string> VectorStoreIds { get; private set; }

        [JsonInclude]
        [JsonPropertyName("vector_stores")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IReadOnlyList<VectorStoreRequest> VectorStores { get; private set; }

        public static implicit operator FileSearchResources(VectorStoreRequest request) => new(request);
    }
}
