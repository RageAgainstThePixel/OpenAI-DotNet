// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using OpenAI.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.VectorStores
{
    /// <summary>
    /// Vector stores are used to store files for use by the file_search tool.
    /// <see href="https://platform.openai.com/docs/api-reference/vector-stores"/>
    /// </summary>
    public sealed class VectorStoresEndpoint : OpenAIBaseEndpoint
    {
        public VectorStoresEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "vector_stores";

        /// <summary>
        /// Creates a new Vector Store.
        /// </summary>
        /// <param name="request"><see cref="CreateVectorStoreRequest"/>.</param>
        /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreResponse"/>.</returns>
        public async Task<VectorStoreResponse> CreateVectorStoreAsync(CreateVectorStoreRequest request, CancellationToken cancellationToken = default)
        {
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl(), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<VectorStoreResponse>(responseAsString, client);
        }

        /// <summary>
        /// Returns a list of vector stores.
        /// </summary>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{VectorStoreResponse}"/>.</returns>
        public async Task<ListResponse<VectorStoreResponse>> ListVectorStoresAsync(ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl(queryParameters: query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<VectorStoreResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Get a vector store.
        /// </summary>
        /// <param name="vectorStoreId">
        /// The ID of the vector store to retrieve.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreResponse"/>.</returns>
        public async Task<VectorStoreResponse> GetVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{vectorStoreId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<VectorStoreResponse>(responseAsString, client);
        }

        /// <summary>
        /// Modifies a vector store.
        /// </summary>
        /// <param name="vectorStoreId">
        /// The ID of the vector store to retrieve.
        /// </param>
        /// <param name="name">
        /// Optional, name of the vector store.
        /// </param>
        /// <param name="expiresAfter">
        /// The number of days after the anchor time that the vector store will expire.
        /// </param>
        /// <param name="metadata">
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreResponse"/>.</returns>
        public async Task<VectorStoreResponse> ModifyVectorStoreAsync(string vectorStoreId, string name = null, int? expiresAfter = null, IReadOnlyDictionary<string, object> metadata = null, CancellationToken cancellationToken = default)
        {
            var expirationPolicy = expiresAfter.HasValue ? new ExpirationPolicy(expiresAfter.Value) : null;
            var request = new { name, expires_after = expirationPolicy, metadata };
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{vectorStoreId}"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<VectorStoreResponse>(responseAsString, client);
        }

        /// <summary>
        /// Delete a vector store.
        /// </summary>
        /// <param name="vectorStoreId">
        /// The ID of the vector store to retrieve.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the vector store was successfully deleted.</returns>
        public async Task<bool> DeleteVectorStoreAsync(string vectorStoreId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.DeleteAsync(GetUrl($"/{vectorStoreId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<DeletedResponse>(responseAsString, client)?.Deleted ?? false;
        }

        #region Files

        /// <summary>
        /// Create a vector store file by attaching a File to a vector store.
        /// </summary>
        /// <param name="vectorStoreId">The ID of the vector store that the file belongs to.</param>
        /// <param name="fileId">
        /// A File ID that the vector store should use.
        /// Useful for tools like file_search that can access files.
        /// </param>
        /// <param name="chunkingStrategy">
        /// A file id that the vector store should use. Useful for tools like 'file_search' that can access files.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreFileResponse"/>.</returns>
        public async Task<VectorStoreFileResponse> CreateVectorStoreFileAsync(string vectorStoreId, string fileId, ChunkingStrategy chunkingStrategy = null, CancellationToken cancellationToken = default)
        {
            using var payload = JsonSerializer.Serialize(new { file_id = fileId, chunking_strategy = chunkingStrategy }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{vectorStoreId}/files"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<VectorStoreFileResponse>(responseAsString, client);
        }

        /// <summary>
        /// Returns a list of vector store files.
        /// </summary>
        /// <param name="vectorStoreId">The ID of the vector store that the file belongs to.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="filter">Optional, Filter by file status <see cref="VectorStoreFileStatus"/> filter.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{VectorStoreFileResponse}"/>.</returns>
        public async Task<ListResponse<VectorStoreFileResponse>> ListVectorStoreFilesAsync(string vectorStoreId, ListQuery query = null, VectorStoreFileStatus? filter = null, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> queryParams = query;

            if (filter.HasValue)
            {
                queryParams ??= new();
                queryParams.Add("filter", $"{filter.Value}");
            }

            using var response = await client.Client.GetAsync(GetUrl($"/{vectorStoreId}/files", queryParams), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<VectorStoreFileResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Retrieves a vector store file.
        /// </summary>
        /// <param name="vectorStoreId">The ID of the vector store that the file belongs to.</param>
        /// <param name="fileId">The ID of the file being retrieved.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreFileResponse"/>.</returns>
        public async Task<VectorStoreFileResponse> GetVectorStoreFileAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{vectorStoreId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<VectorStoreFileResponse>(responseAsString, client);
        }

        /// <summary>
        /// Delete a vector store file.
        /// This will remove the file from the vector store but the file itself will not be deleted.
        /// To delete the file, use the delete file endpoint.
        /// </summary>
        /// <param name="vectorStoreId">The ID of the vector store that the file belongs to.</param>
        /// <param name="fileId">The ID of the file being deleted.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the vector store file was successfully deleted.</returns>
        public async Task<bool> DeleteVectorStoreFileAsync(string vectorStoreId, string fileId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.DeleteAsync(GetUrl($"/{vectorStoreId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<DeletedResponse>(responseAsString, client)?.Deleted ?? false;
        }

        #endregion Files

        #region Batches

        /// <summary>
        /// Create a vector store file batch.
        /// </summary>
        /// <param name="vectorStoreId">
        /// The ID of the vector store for which to create a File Batch.
        /// </param>
        /// <param name="fileIds">
        /// A list of File IDs that the vector store should use. Useful for tools like file_search that can access files.
        /// </param>
        /// <param name="chunkingStrategy">
        /// A file id that the vector store should use. Useful for tools like 'file_search' that can access files.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreFileBatchResponse"/>.</returns>
        public async Task<VectorStoreFileBatchResponse> CreateVectorStoreFileBatchAsync(string vectorStoreId, IReadOnlyList<string> fileIds, ChunkingStrategy chunkingStrategy = null, CancellationToken cancellationToken = default)
        {
            if (fileIds is not { Count: not 0 }) { throw new ArgumentNullException(nameof(fileIds)); }
            using var payload = JsonSerializer.Serialize(new { file_ids = fileIds, chunking_strategy = chunkingStrategy }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{vectorStoreId}/file_batches"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<VectorStoreFileBatchResponse>(responseAsString, client);
        }

        /// <summary>
        /// Create a vector store file batch.
        /// </summary>
        /// <param name="vectorStoreId">
        /// The ID of the vector store for which to create a File Batch.
        /// </param>
        /// <param name="files">
        /// A list of Files that the vector store should use. Useful for tools like file_search that can access files.
        /// </param>
        /// <param name="chunkingStrategy">
        /// A file id that the vector store should use. Useful for tools like 'file_search' that can access files.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreFileBatchResponse"/>.</returns>
        public async Task<VectorStoreFileBatchResponse> CreateVectorStoreFileBatchAsync(string vectorStoreId, IReadOnlyList<FileResponse> files, ChunkingStrategy chunkingStrategy = null, CancellationToken cancellationToken = default)
            => await CreateVectorStoreFileBatchAsync(vectorStoreId, files?.Select(file => file.Id).ToList(), chunkingStrategy, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns a list of vector store files in a batch.
        /// </summary>
        /// <param name="vectorStoreId">The ID of the vector store that the files belong to.</param>
        /// <param name="fileBatchId">The ID of the file batch.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="filter">Optional, filter by file status.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{VectorStoreFileBatch}"/>.</returns>
        public async Task<ListResponse<VectorStoreFileBatchResponse>> ListVectorStoreBatchFilesAsync(string vectorStoreId, string fileBatchId, ListQuery query = null, VectorStoreFileStatus? filter = null, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> queryParams = query;

            if (filter != null)
            {
                queryParams ??= new();
                queryParams.Add("filter", $"{filter.Value}");
            }

            using var response = await client.Client.GetAsync(GetUrl($"/{vectorStoreId}/file_batches/{fileBatchId}/files", queryParams), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<VectorStoreFileBatchResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Retrieves a vector store file batch.
        /// </summary>
        /// <param name="vectorStoreId">The ID of the vector store that the files belong to.</param>
        /// <param name="fileBatchId">The ID of the file batch being retrieved.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreFileBatchResponse"/>.</returns>
        public async Task<VectorStoreFileBatchResponse> GetVectorStoreFileBatchAsync(string vectorStoreId, string fileBatchId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{vectorStoreId}/file_batches/{fileBatchId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<VectorStoreFileBatchResponse>(responseAsString, client);
        }

        /// <summary>
        /// Cancel a vector store file batch.
        /// This attempts to cancel the processing of files in this batch as soon as possible.
        /// </summary>
        /// <param name="vectorStoreId">The ID of the vector store that the files belong to.</param>
        /// <param name="fileBatchId">The ID of the file batch being retrieved.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the vector store file batch was cancelled, otherwise false.</returns>
        public async Task<bool> CancelVectorStoreFileBatchAsync(string vectorStoreId, string fileBatchId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.PostAsync(GetUrl($"/{vectorStoreId}/file_batches/{fileBatchId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var result = response.Deserialize<VectorStoreFileBatchResponse>(responseAsString, client);

            if (result.Status < VectorStoreFileStatus.Cancelling)
            {
                try
                {
                    result = await result.WaitForStatusChangeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return result.Status >= VectorStoreFileStatus.Cancelling;
        }

        #endregion Batches
    }
}
