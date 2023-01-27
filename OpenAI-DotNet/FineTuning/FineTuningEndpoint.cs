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
    /// Manage fine-tuning jobs to tailor a model to your specific training data.
    /// <see href="https://beta.openai.com/docs/guides/fine-tuning"/>
    /// </summary>
    public class FineTuningEndpoint : BaseEndPoint
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
        protected override string GetEndpoint()
            => $"{Api.BaseUrl}fine-tunes";

        /// <summary>
        /// Creates a job that fine-tunes a specified model from a given dataset.
        /// Response includes details of the enqueued job including job status and
        /// the name of the fine-tuned models once complete.
        /// </summary>
        /// <param name="jobRequest"><see cref="CreateFineTuneJobRequest"/>.</param>
        /// <returns><see cref="FineTuneJob"/>.</returns>
        /// <exception cref="HttpRequestException">.</exception>
        public async Task<FineTuneJobResponse> CreateFineTuneAsync(CreateFineTuneJobRequest jobRequest)
        {
            var jsonContent = JsonSerializer.Serialize(jobRequest, Api.JsonSerializationOptions);
            var response = await Api.Client.PostAsync(GetEndpoint(), jsonContent.ToJsonStringContent());
            var responseAsString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{nameof(CreateFineTuneAsync)} Failed! HTTP status code: {response.StatusCode}. Request body: {responseAsString}");
            }

            var result = JsonSerializer.Deserialize<FineTuneJobResponse>(responseAsString, Api.JsonSerializationOptions);
            result.SetResponseData(response.Headers);
            return result;
        }

        /// <summary>
        /// List your organization's fine-tuning jobs.
        /// </summary>
        /// <returns>List of <see cref="FineTuneJob"/>s.</returns>
        /// <exception cref="HttpRequestException">.</exception>
        public async Task<IReadOnlyList<FineTuneJob>> ListFineTuneJobsAsync()
        {
            var response = await Api.Client.GetAsync(GetEndpoint());
            var responseAsString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{nameof(ListFineTuneJobsAsync)} Failed! HTTP status code: {response.StatusCode}. Request body: {responseAsString}");
            }

            return JsonSerializer.Deserialize<FineTuneList>(responseAsString, Api.JsonSerializationOptions)?.Data.OrderBy(job => job.CreatedAtUnixTime).ToArray();
        }

        /// <summary>
        /// Gets info about the fine-tune job.
        /// </summary>
        /// <param name="fineTuneJob"><see cref="FineTuneJob"/>.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        /// <exception cref="HttpRequestException">.</exception>
        public async Task<FineTuneJobResponse> RetrieveFineTuneJobInfoAsync(FineTuneJob fineTuneJob)
        {
            var response = await Api.Client.GetAsync($"{GetEndpoint()}/{fineTuneJob.Id}");
            var responseAsString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{nameof(RetrieveFineTuneJobInfoAsync)} Failed! HTTP status code: {response.StatusCode}. Request body: {responseAsString}");
            }

            var result = JsonSerializer.Deserialize<FineTuneJobResponse>(responseAsString, Api.JsonSerializationOptions);
            result.SetResponseData(response.Headers);
            return result;
        }

        /// <summary>
        /// Immediately cancel a fine-tune job.
        /// </summary>
        /// <param name="fineTuneJob"><see cref="FineTuneJob"/> to cancel.</param>
        /// <returns><see cref="FineTuneJobResponse"/>.</returns>
        /// <exception cref="HttpRequestException">.</exception>
        public async Task<bool> CancelFineTuneJob(FineTuneJob fineTuneJob)
        {
            var response = await Api.Client.PostAsync($"{GetEndpoint()}/{fineTuneJob.Id}/cancel", null);
            var responseAsString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{nameof(CancelFineTuneJob)} Failed! HTTP status code: {response.StatusCode}. Request body: {responseAsString}");
            }

            var result = JsonSerializer.Deserialize<FineTuneJobResponse>(responseAsString, Api.JsonSerializationOptions);
            result.SetResponseData(response.Headers);
            return result.Status == "cancelled";
        }

        /// <summary>
        /// Get fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="fineTuneJob"><see cref="FineTuneJob"/>.</param>
        /// <returns>List of events for <see cref="FineTuneJob"/>.</returns>
        public async Task<IReadOnlyList<Event>> ListFineTuneEventsAsync(FineTuneJob fineTuneJob)
        {
            var response = await Api.Client.GetAsync($"{GetEndpoint()}/{fineTuneJob.Id}/events");
            var responseAsString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{nameof(ListFineTuneEventsAsync)} Failed! HTTP status code: {response.StatusCode}. Request body: {responseAsString}");
            }

            return JsonSerializer.Deserialize<FineTuneEventList>(responseAsString, Api.JsonSerializationOptions)?.Data.OrderBy(@event => @event.CreatedAtUnixTime).ToArray();
        }

        /// <summary>
        /// Stream the fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="fineTuneJob"><see cref="FineTuneJob"/>.</param>
        /// <param name="fineTuneEventCallback">The event callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <exception cref="HttpRequestException"></exception>
        public async Task StreamFineTuneEventsAsync(FineTuneJob fineTuneJob, Action<Event> fineTuneEventCallback, CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{GetEndpoint()}/{fineTuneJob.Id}/events?stream=true");
            var response = await Api.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream);

                while (await reader.ReadLineAsync() is { } line &&
                       !cancellationToken.IsCancellationRequested)
                {
                    if (line.StartsWith("data: "))
                    {
                        line = line["data: ".Length..];
                    }

                    if (line == "[DONE]")
                    {
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        fineTuneEventCallback(JsonSerializer.Deserialize<Event>(line.Trim()));
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    var result = await CancelFineTuneJob(fineTuneJob);

                    if (!result)
                    {
                        throw new Exception($"Failed to cancel {fineTuneJob}");
                    }
                }
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);
                throw new HttpRequestException($"{nameof(StreamFineTuneEventsAsync)} Failed! HTTP status code: {response.StatusCode}. Request body: {responseBody}");
            }
        }

        /// <summary>
        /// Stream the fine-grained status updates for a fine-tune job.
        /// </summary>
        /// <param name="fineTuneJob"><see cref="FineTuneJob"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <exception cref="HttpRequestException"></exception>
        public async IAsyncEnumerable<Event> StreamFineTuneEventsEnumerableAsync(FineTuneJob fineTuneJob, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{GetEndpoint()}/{fineTuneJob.Id}/events?stream=true");
            var response = await Api.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream);

                while (await reader.ReadLineAsync() is { } line &&
                       !cancellationToken.IsCancellationRequested)
                {
                    if (line.StartsWith("data: "))
                    {
                        line = line["data: ".Length..];
                    }

                    if (line == "[DONE]")
                    {
                        yield break;
                    }

                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        yield return JsonSerializer.Deserialize<Event>(line.Trim());
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    var result = await CancelFineTuneJob(fineTuneJob);

                    if (!result)
                    {
                        throw new Exception($"Failed to cancel {fineTuneJob}");
                    }
                }
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);
                throw new HttpRequestException($"{nameof(StreamFineTuneEventsEnumerableAsync)} Failed! HTTP status code: {response.StatusCode}. Request body: {responseBody}");
            }
        }
    }
}
