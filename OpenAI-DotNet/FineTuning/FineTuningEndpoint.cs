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
            using var jsonContent = JsonSerializer.Serialize(jobRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl("/jobs"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<FineTuneJobResponse>(responseAsString, client);
        }

        /// <summary>
        /// List your organization's fine-tuning jobs.
        /// </summary>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>List of <see cref="FineTuneJobResponse"/>s.</returns>
        public async Task<ListResponse<FineTuneJobResponse>> ListJobsAsync(ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl("/jobs", query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<FineTuneJobResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Gets info about the fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        public async Task<FineTuneJobResponse> GetJobInfoAsync(string jobId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/jobs/{jobId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            var job = response.Deserialize<FineTuneJobResponse>(responseAsString, client);
            job.Events = (await ListJobEventsAsync(job, query: null, cancellationToken: cancellationToken).ConfigureAwait(false))?.Items;
            return job;
        }

        /// <summary>
        /// Immediately cancel a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/> to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        public async Task<bool> CancelJobAsync(string jobId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.PostAsync(GetUrl($"/jobs/{jobId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<FineTuneJobResponse>(responseAsString, OpenAIClient.JsonSerializationOptions);
            return result.Status == JobStatus.Cancelled;
        }

        /// <summary>
        /// Get fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>List of events for <see cref="FineTuneJobResponse"/>.</returns>
        public async Task<ListResponse<EventResponse>> ListJobEventsAsync(string jobId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/jobs/{jobId}/events", query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<EventResponse>>(responseAsString, client);
        }
    }
}
