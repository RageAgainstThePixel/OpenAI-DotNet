// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Batch
{
    /// <summary>
    /// Create large batches of API requests for asynchronous processing.
    /// The Batch API returns completions within 24 hours for a 50% discount.
    /// <see href="https://platform.openai.com/docs/api-reference/batch"/>
    /// </summary>
    public class BatchEndpoint : OpenAIBaseEndpoint
    {
        public BatchEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "batches";

        /// <summary>
        /// Creates and executes a batch from an uploaded file of requests.
        /// </summary>
        /// <param name="inputFileId">
        /// The ID of an uploaded file that contains requests for the new batch.
        /// Your input file must be formatted as a JSONL file, and must be uploaded with the purpose batch.
        /// The file can contain up to 50,000 requests, and can be up to 100 MB in size.
        /// </param>
        /// <param name="endpoint">
        /// The endpoint to be used for all requests in the batch.
        /// Currently, /v1/chat/completions, /v1/embeddings, and /v1/completions are supported.
        /// Note that /v1/embeddings batches are also restricted to a maximum of 50,000 embedding inputs across all requests in the batch.
        /// </param>
        /// <param name="metadata">
        /// Optional custom metadata for the batch.
        /// </param>
        /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BatchResponse"/>.</returns>
        public async Task<BatchResponse> CreateBatchAsync(string inputFileId, string endpoint, IReadOnlyDictionary<string, object> metadata = null, CancellationToken cancellationToken = default)
        {
            // ReSharper disable once InconsistentNaming
            const string completion_window = "24h";
            var request = new { input_file_id = inputFileId, endpoint, completion_window };
            using var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl(), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<BatchResponse>(responseAsString, client);
        }

        /// <summary>
        /// List your organization's batches.
        /// </summary>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{BatchResponse}"/>.</returns>
        public async Task<ListResponse<BatchResponse>> ListBatchesAsync(ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl(queryParameters: query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<BatchResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Retrieves a batch.
        /// </summary>
        /// <param name="batchId">The ID of the batch to retrieve.</param>
        /// <param name="cancellationToken"> Optional <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BatchResponse"/>.</returns>
        public async Task<BatchResponse> RetrieveBatchAsync(string batchId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{batchId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<BatchResponse>(responseAsString, client);
        }

        /// <summary>
        /// Cancels an in-progress batch.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="cancellationToken"> Optional <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the batch was cancelled, otherwise false.</returns>
        public async Task<bool> CancelBatchAsync(string batchId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.PostAsync(GetUrl($"/{batchId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<BatchResponse>(responseAsString, OpenAIClient.JsonSerializationOptions);
            return result.Status == BatchStatus.Cancelled;
        }
    }
}
