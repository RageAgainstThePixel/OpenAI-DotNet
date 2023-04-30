using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        private class FineTuneList
        {
            [JsonPropertyName("object")]
            public string Object { get; set; }

            [JsonPropertyName("data")]
            public List<FineTuneJob> Data { get; set; }
        }

        private class FineTuneEventList
        {
            [JsonPropertyName("data")]
            public List<Event> Data { get; set; }
        }

        /// <inheritdoc />
        public FineTuningEndpoint(OpenAIClient api) : base(api) { }

        /// <inheritdoc />
        protected override string Root => "fine-tunes";

        /// <summary>
        /// Creates a job that fine-tunes a specified model from a given dataset.
        /// Response includes details of the enqueued job including job status and
        /// the name of the fine-tuned models once complete.
        /// </summary>
        /// <param name="jobRequest"><see cref="CreateFineTuneJobRequest"/>.</param>
        /// <returns><see cref="FineTuneJob"/>.</returns>
        /// <exception cref="HttpRequestException">.</exception>
        public async Task<FineTuneJob> CreateFineTuneJobAsync(CreateFineTuneJobRequest jobRequest)
        {
            var jsonContent = JsonSerializer.Serialize(jobRequest, Api.JsonSerializationOptions).ToJsonStringContent();
            var response = await Api.Client.PostAsync(GetUrl(), jsonContent).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            return response.DeserializeResponse<FineTuneJobResponse>(responseAsString, Api.JsonSerializationOptions);
        }

        /// <summary>
        /// List your organization's fine-tuning jobs.
        /// </summary>
        /// <returns>List of <see cref="FineTuneJob"/>s.</returns>
        /// <exception cref="HttpRequestException">.</exception>
        public async Task<IReadOnlyList<FineTuneJob>> ListFineTuneJobsAsync()
        {
            var response = await Api.Client.GetAsync(GetUrl()).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<FineTuneList>(responseAsString, Api.JsonSerializationOptions)?.Data.OrderBy(job => job.CreatedAtUnixTime).ToArray();
        }

        /// <summary>
        /// Gets info about the fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<FineTuneJob> RetrieveFineTuneJobInfoAsync(string jobId)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{jobId}")).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            return response.DeserializeResponse<FineTuneJobResponse>(responseAsString, Api.JsonSerializationOptions);
        }

        /// <summary>
        /// Immediately cancel a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/> to cancel.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<bool> CancelFineTuneJobAsync(string jobId)
        {
            var response = await Api.Client.PostAsync(GetUrl($"/{jobId}/cancel"), null!).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync().ConfigureAwait(false);
            var result = response.DeserializeResponse<FineTuneJobResponse>(responseAsString, Api.JsonSerializationOptions);
            const string cancelled = "cancelled";
            return result.Status == cancelled;
        }

        /// <summary>
        /// Get fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>List of events for <see cref="FineTuneJob"/>.</returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<IReadOnlyList<Event>> ListFineTuneEventsAsync(string jobId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{jobId}/events"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<FineTuneEventList>(responseAsString, Api.JsonSerializationOptions)?.Data.OrderBy(@event => @event.CreatedAtUnixTime).ToArray();
        }

        /// <summary>
        /// Stream the fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
        /// <param name="fineTuneEventCallback">The event callback handler.</param>
        /// <param name="cancelJob">Optional, Cancel the job if streaming is aborted. Default is false.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <exception cref="HttpRequestException"></exception>
        public async Task StreamFineTuneEventsAsync(string jobId, Action<Event> fineTuneEventCallback, bool cancelJob = false, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, GetUrl($"/{jobId}/events?stream=true"));
            var response = await Api.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);

            while (!cancellationToken.IsCancellationRequested &&
                   await reader.ReadLineAsync().ConfigureAwait(false) is { } streamData)
            {
                if (streamData.TryGetEventStreamData(out var eventData))
                {
                    if (string.IsNullOrWhiteSpace(eventData)) { continue; }
                    fineTuneEventCallback(JsonSerializer.Deserialize<Event>(eventData, Api.JsonSerializationOptions));
                }
                else
                {
                    break;
                }
            }

            if (cancellationToken.IsCancellationRequested && cancelJob)
            {
                var isCancelled = await CancelFineTuneJobAsync(jobId).ConfigureAwait(false);

                if (!isCancelled)
                {
                    throw new Exception($"Failed to cancel {jobId}");
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Stream the fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="jobId"><see cref="FineTuneJob.Id"/>.</param>
        /// <param name="cancelJob">Optional, Cancel the job if streaming is aborted. Default is false.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <exception cref="HttpRequestException"></exception>
        public async IAsyncEnumerable<Event> StreamFineTuneEventsEnumerableAsync(string jobId, bool cancelJob = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, GetUrl($"/{jobId}/events?stream=true"));
            var response = await Api.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);

            while (!cancellationToken.IsCancellationRequested &&
                   await reader.ReadLineAsync().ConfigureAwait(false) is { } streamData)
            {
                if (streamData.TryGetEventStreamData(out var eventData))
                {
                    if (string.IsNullOrWhiteSpace(eventData)) { continue; }
                    yield return JsonSerializer.Deserialize<Event>(eventData, Api.JsonSerializationOptions);
                }
                else
                {
                    break;
                }
            }

            if (cancellationToken.IsCancellationRequested && cancelJob)
            {
                var isCancelled = await CancelFineTuneJobAsync(jobId).ConfigureAwait(false);

                if (!isCancelled)
                {
                    throw new Exception($"Failed to cancel {jobId}");
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
