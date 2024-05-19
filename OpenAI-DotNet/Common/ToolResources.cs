// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// A set of resources to be used by Assistants and Threads.
    /// The resources are specific to the type of tool.
    /// For example, the <see cref="Tool.CodeInterpreter"/> requres a list of file ids,
    /// While the <see cref="Tool.FileSearch"/> requires a list vector store ids.
    /// </summary>
    public class ToolResources
    {
        public ToolResources(
            CodeInterpreterResources codeInterpreter = null,
            FileSearchResources fileSearch = null)
        {
            CodeInterpreter = codeInterpreter;
            FileSearch = fileSearch;
        }

        [JsonInclude]
        [JsonPropertyName("code_interpreter")]
        public CodeInterpreterResources CodeInterpreter { get; private set; }

        [JsonInclude]
        [JsonPropertyName("file_search")]
        public FileSearchResources FileSearch { get; private set; }
    }

    /// <summary>
    /// <see cref="Tool.CodeInterpreter"/> resources.
    /// </summary>
    public class CodeInterpreterResources
    {
        public CodeInterpreterResources(IReadOnlyList<string> fileIds)
        {
            FileIds = fileIds;
        }

        /// <summary>
        /// A list of file IDs made available to the <see cref="Tool.CodeInterpreter"/> tool.
        /// There can be a maximum of 20 files associated with the tool.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("file_ids")]
        public IReadOnlyList<string> FileIds { get; private set; }
    }

    /// <summary>
    /// <see cref="Tool.FileSearch"/> resources.
    /// </summary>
    public class FileSearchResources
    {
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
        public IReadOnlyList<string> VectorStoreIds { get; private set; }

        [JsonInclude]
        [JsonPropertyName("vector_stores")]
        public IReadOnlyList<VectorStoreRequest> VectorStores { get; private set; }
    }

    /// <summary>
    /// A helper to create a vector store with file_ids and attach it to an assistant/thread.
    /// There can be a maximum of 1 vector store attached to the assistant/thread.
    /// </summary>
    public class VectorStoreRequest
    {
        public VectorStoreRequest(IReadOnlyList<string> fileIds, IReadOnlyDictionary<string, string> metadata)
        {
            FileIds = fileIds;
            Metadata = metadata;
        }

        [JsonInclude]
        [JsonPropertyName("file_ids")]
        public IReadOnlyList<string> FileIds { get; private set; }

        [JsonInclude]
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }
    }
}
