// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Batch
{
    public static class BatchExtensions
    {
        /// <summary>
        /// Get the latest status of the <see cref="BatchResponse"/>.
        /// </summary>
        /// <param name="batchResponse"><see cref="BatchResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BatchResponse"/>.</returns>
        public static async Task<BatchResponse> UpdateAsync(this BatchResponse batchResponse, CancellationToken cancellationToken = default)
            => await batchResponse.Client.BatchEndpoint.RetrieveBatchAsync(batchResponse.Id, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Waits for <see cref="BatchResponse.Status"/> to change.
        /// </summary>
        /// <param name="batchResponse"><see cref="BatchResponse"/>.</param>
        /// <param name="pollingInterval">Optional, time in milliseconds to wait before polling status.</param>
        /// <param name="timeout">Optional, timeout in seconds to cancel polling.<br/>Defaults to 30 seconds.<br/>Set to -1 for indefinite.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="BatchResponse"/>.</returns>
        public static async Task<BatchResponse> WaitForStatusChangeAsync(this BatchResponse batchResponse, int? pollingInterval = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource cts = timeout is < 0
                ? new CancellationTokenSource()
                : new CancellationTokenSource(TimeSpan.FromSeconds(timeout ?? 30));
            using var chainedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
            BatchResponse result;
            do
            {
                await Task.Delay(pollingInterval ?? 500, chainedCts.Token).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                result = await batchResponse.UpdateAsync(cancellationToken: chainedCts.Token).ConfigureAwait(false);
            } while (result.Status is BatchStatus.NotStarted or BatchStatus.InProgress or BatchStatus.Cancelling);
            return result;
        }
    }
}
