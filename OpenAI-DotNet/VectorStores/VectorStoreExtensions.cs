// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.VectorStores
{
    public static class VectorStoreExtensions
    {
        /// <summary>
        /// Get the latest status of the <see cref="VectorStoreFileBatchResponse"/>.
        /// </summary>
        /// <param name="vectorStoreFileBatchResponse"><see cref="VectorStoreFileBatchResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreFileBatchResponse"/>.</returns>
        public static async Task<VectorStoreFileBatchResponse> UpdateAsync(this VectorStoreFileBatchResponse vectorStoreFileBatchResponse, CancellationToken cancellationToken = default)
            => await vectorStoreFileBatchResponse.Client.VectorStoresEndpoint.GetVectorStoreFileBatchAsync(vectorStoreFileBatchResponse.VectorStoreId, vectorStoreFileBatchResponse.Id, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Waits for <see cref="VectorStoreFileBatchResponse.Status"/> to change.
        /// </summary>
        /// <param name="vectorStoreFileBatchResponse"><see cref="VectorStoreFileBatchResponse"/>.</param>
        /// <param name="pollingInterval">Optional, time in milliseconds to wait before polling status.</param>
        /// <param name="timeout">Optional, timeout in seconds to cancel polling.<br/>Defaults to 30 seconds.<br/>Set to -1 for indefinite.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="VectorStoreFileBatchResponse"/>.</returns>
        public static async Task<VectorStoreFileBatchResponse> WaitForStatusChangeAsync(this VectorStoreFileBatchResponse vectorStoreFileBatchResponse, int? pollingInterval = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource cts = timeout is < 0
                ? new CancellationTokenSource()
                : new CancellationTokenSource(TimeSpan.FromSeconds(timeout ?? 30));
            using var chainedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
            VectorStoreFileBatchResponse result;
            do
            {
                await Task.Delay(pollingInterval ?? 500, chainedCts.Token).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                result = await vectorStoreFileBatchResponse.UpdateAsync(cancellationToken: chainedCts.Token).ConfigureAwait(false);
            } while (result.Status is VectorStoreFileStatus.NotStarted or VectorStoreFileStatus.InProgress or VectorStoreFileStatus.Cancelling);
            return result;
        }
    }
}
