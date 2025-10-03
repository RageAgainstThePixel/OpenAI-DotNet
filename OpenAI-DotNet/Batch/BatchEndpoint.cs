// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
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
    public sealed class BatchEndpoint : OpenAIBaseEndpoint
    {
        public BatchEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "batches";

        /// <summary>
        /// Creates and executes a batch from an uploaded file of requests.
        /// </summary>
        /// <param name="request"><see cref="CreateBatchRequest"/>.</param>
        /// <param name="cancellationToken">Optional <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BatchResponse"/>.</returns>
        public async Task<BatchResponse> CreateBatchAsync(CreateBatchRequest request, CancellationToken cancellationToken = default)
        {
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await PostAsync(GetUrl(), payload, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<BatchResponse>(EnableDebug, payload, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List your organization's batches.
        /// </summary>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{BatchResponse}"/>.</returns>
        public async Task<ListResponse<BatchResponse>> ListBatchesAsync(ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await GetAsync(GetUrl(queryParameters: query), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<ListResponse<BatchResponse>>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a batch.
        /// </summary>
        /// <param name="batchId">The ID of the batch to retrieve.</param>
        /// <param name="cancellationToken"> Optional <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BatchResponse"/>.</returns>
        public async Task<BatchResponse> RetrieveBatchAsync(string batchId, CancellationToken cancellationToken = default)
        {
            using var response = await GetAsync(GetUrl($"/{batchId}"), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<BatchResponse>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Cancels an in-progress batch.
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="cancellationToken"> Optional <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the batch was cancelled, otherwise false.</returns>
        public async Task<bool> CancelBatchAsync(string batchId, CancellationToken cancellationToken = default)
        {
            using var response = await PostAsync(GetUrl($"/{batchId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            var batch = await response.DeserializeAsync<BatchResponse>(EnableDebug, client, cancellationToken).ConfigureAwait(false);

            if (batch.Status < BatchStatus.Cancelling)
            {
                try
                {
                    batch = await batch.WaitForStatusChangeAsync(cancellationToken: cancellationToken);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return batch.Status >= BatchStatus.Cancelling;
        }
    }
}
