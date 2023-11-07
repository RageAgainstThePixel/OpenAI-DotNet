using OpenAI.Extensions;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.FineTuning
{
    /// <summary>
    /// Manage fine-tuning jobs to tailor a model to your specific training data.<br/>
    /// <see href="https://platform.openai.com/docs/guides/fine-tuning"/>
    /// </summary>
    public sealed class FineTuningEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        public FineTuningEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string Root => "fine_tuning";

        /// <summary>
        /// Creates a job that fine-tunes a specified model from a given dataset.
        /// Response includes details of the queued job including job status and
        /// the name of the fine-tuned models once complete.
        /// </summary>
        /// <param name="jobRequest"><see cref="CreateFineTuneJobRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FineTuneJob"/>.</returns>
        /// <exception cref="HttpRequestException">.</exception>
        public async Task<FineTuneJob> CreateJobAsync(CreateFineTuneJobRequest jobRequest, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(jobRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl("/jobs"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<FineTuneJob>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// List your organization's fine-tuning jobs.
        /// </summary>
        /// <param name="limit">Number of fine-tuning jobs to retrieve (Default 20).</param>
        /// <param name="after">Identifier for the last job from the previous pagination request.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>List of <see cref="FineTuneJob"/>s.</returns>
        /// <exception cref="HttpRequestException">.</exception>
        public async Task<FineTuneJobList> ListJobsAsync(int? limit = null, string after = null, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>();

            if (limit.HasValue)
            {
                parameters.Add(nameof(limit), limit.ToString());
            }

            if (!string.IsNullOrWhiteSpace(after))
            {
                parameters.Add(nameof(after), after);
            }

            var response = await Api.Client.GetAsync(GetUrl("/jobs", parameters), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<FineTuneJobList>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Gets info about the fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FineTuneJob"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<FineTuneJob> GetJobInfoAsync(string jobId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/jobs/{jobId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var job = JsonSerializer.Deserialize<FineTuneJob>(responseAsString, OpenAIClient.JsonSerializationOptions);
            job.Events = (await ListJobEventsAsync(job, cancellationToken: cancellationToken).ConfigureAwait(false)).Events;
            return job;
        }

        /// <summary>
        /// Immediately cancel a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/> to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="FineTuneJob"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<bool> CancelJobAsync(string jobId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.PostAsync(GetUrl($"/jobs/{jobId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<FineTuneJob>(responseAsString, OpenAIClient.JsonSerializationOptions);
            return result.Status == JobStatus.Cancelled;
        }

        /// <summary>
        /// Get fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
        /// <param name="limit">Number of fine-tuning jobs to retrieve (Default 20).</param>
        /// <param name="after">Identifier for the last <see cref="Event"/> from the previous pagination request.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>List of events for <see cref="FineTuneJob"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<EventList> ListJobEventsAsync(string jobId, int? limit = null, string after = null, CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>();

            if (limit.HasValue)
            {
                parameters.Add(nameof(limit), limit.ToString());
            }

            if (!string.IsNullOrWhiteSpace(after))
            {
                parameters.Add(nameof(after), after);
            }

            var response = await Api.Client.GetAsync(GetUrl($"/jobs/{jobId}/events", parameters), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<EventList>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }
    }
}
