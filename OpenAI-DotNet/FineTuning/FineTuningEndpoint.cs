// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.FineTuning
{
    /// <summary>
    /// Manage fine-tuning jobs to tailor a model to your specific training data.<br/>
    /// <see href="https://platform.openai.com/docs/guides/fine-tuning"/><br/>
    /// <see href="https://platform.openai.com/docs/api-reference/fine-tuning"/>
    /// </summary>
    public sealed class FineTuningEndpoint : OpenAIBaseEndpoint
    {
        /// <inheritdoc />
        public FineTuningEndpoint(OpenAIClient client) : base(client) { }

        /// <inheritdoc />
        protected override string Root => "fine_tuning";

        /// <summary>
        /// Creates a job that fine-tunes a specified model from a given dataset.
        /// Response includes details of the queued job including job status and
        /// the name of the fine-tuned models once complete.
        /// </summary>
        /// <param name="jobRequest"><see cref="CreateFineTuneJobRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        public async Task<FineTuneJobResponse> CreateJobAsync(CreateFineTuneJobRequest jobRequest, CancellationToken cancellationToken = default)
        {
            using var payload = JsonSerializer.Serialize(jobRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await PostAsync(GetUrl("/jobs"), payload, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<FineTuneJobResponse>(EnableDebug, payload, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// List your organization's fine-tuning jobs.
        /// </summary>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>List of <see cref="FineTuneJobResponse"/>s.</returns>
        public async Task<ListResponse<FineTuneJobResponse>> ListJobsAsync(ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await GetAsync(GetUrl("/jobs", query), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<ListResponse<FineTuneJobResponse>>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets info about the fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJobResponse.Id"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        public async Task<FineTuneJobResponse> GetJobInfoAsync(string jobId, CancellationToken cancellationToken = default)
        {
            using var response = await GetAsync(GetUrl($"/jobs/{jobId}"), cancellationToken).ConfigureAwait(false);
            var job = await response.DeserializeAsync<FineTuneJobResponse>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
            var jobs = await ListJobEventsAsync(job, null, cancellationToken).ConfigureAwait(false);
            job.Events = jobs?.Items;
            return job;
        }

        /// <summary>
        /// Immediately cancel a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJobResponse.Id"/> to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        public async Task<bool> CancelJobAsync(string jobId, CancellationToken cancellationToken = default)
        {
            using var response = await PostAsync(GetUrl($"/jobs/{jobId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            var result = await response.DeserializeAsync<FineTuneJobResponse>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
            return result.Status == JobStatus.Cancelled;
        }

        /// <summary>
        /// Get fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJobResponse.Id"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>List of events for <see cref="FineTuneJobResponse"/>.</returns>
        public async Task<ListResponse<EventResponse>> ListJobEventsAsync(string jobId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await GetAsync(GetUrl($"/jobs/{jobId}/events", query), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<ListResponse<EventResponse>>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }
    }
}
