using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.FineTuning;

public interface IFineTuningEndpoint
{
    /// <summary>
    /// Creates a job that fine-tunes a specified model from a given dataset.
    /// Response includes details of the enqueued job including job status and
    /// the name of the fine-tuned models once complete.
    /// </summary>
    /// <param name="jobRequest"><see cref="CreateFineTuneJobRequest"/>.</param>
    /// <returns><see cref="FineTuneJob"/>.</returns>
    /// <exception cref="HttpRequestException">.</exception>
    Task<FineTuneJob> CreateFineTuneJobAsync(CreateFineTuneJobRequest jobRequest);

    /// <summary>
    /// List your organization's fine-tuning jobs.
    /// </summary>
    /// <returns>List of <see cref="FineTuneJob"/>s.</returns>
    /// <exception cref="HttpRequestException">.</exception>
    Task<IReadOnlyList<FineTuneJob>> ListFineTuneJobsAsync();

    /// <summary>
    /// Gets info about the fine-tune job.
    /// </summary>
    /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
    /// <returns><see cref="FineTuneJobResponse"/>.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<FineTuneJob> RetrieveFineTuneJobInfoAsync(string jobId);

    /// <summary>
    /// Immediately cancel a fine-tune job.
    /// </summary>
    /// <param name="jobId"><see cref="FineTuneJob.Id"/> to cancel.</param>
    /// <returns><see cref="FineTuneJobResponse"/>.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<bool> CancelFineTuneJobAsync(string jobId);

    /// <summary>
    /// Get fine-grained status updates for a fine-tune job.
    /// </summary>
    /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <returns>List of events for <see cref="FineTuneJob"/>.</returns>
    /// <exception cref="HttpRequestException"></exception>
    Task<IReadOnlyList<Event>> ListFineTuneEventsAsync(string jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream the fine-grained status updates for a fine-tune job.
    /// </summary>
    /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
    /// <param name="fineTuneEventCallback">The event callback handler.</param>
    /// <param name="cancelJob">Optional, Cancel the job if streaming is aborted. Default is false.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <exception cref="HttpRequestException"></exception>
    Task StreamFineTuneEventsAsync(string jobId, Action<Event> fineTuneEventCallback, bool cancelJob = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stream the fine-grained status updates for a fine-tune job.
    /// </summary>
    /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
    /// <param name="cancelJob">Optional, Cancel the job if streaming is aborted. Default is false.</param>
    /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
    /// <exception cref="HttpRequestException"></exception>
    IAsyncEnumerable<Event> StreamFineTuneEventsEnumerableAsync(string jobId, bool cancelJob = false, CancellationToken cancellationToken = default);
}