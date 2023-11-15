using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Extensions;

namespace OpenAI.Threads
{
    public class ThreadsEndpoint : BaseEndPoint
    {
        public ThreadsEndpoint(OpenAIClient api) : base(api)
        {
        }

        protected override string Root => "/threads";

        /// <summary>
        /// Create a thread.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>A thread object.</returns>
        public async Task<Thread> CreateThreadAsync(CreateThreadRequest request,
            CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions)
                .ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl(), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var created = JsonSerializer.Deserialize<Thread>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return created;
        }

        /// <summary>
        /// Retrieves a thread.
        /// </summary>
        /// <param name="threadId">The ID of the thread to retrieve.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The thread object matching the specified ID.</returns>
        public async Task<Thread> RetrieveThreadAsync(string threadId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var thread = JsonSerializer.Deserialize<Thread>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return thread;
        }

        /// <summary>
        /// Modifies a thread.
        /// </summary>
        /// <param name="threadId">The ID of the thread to modify. Only the metadata can be modified.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The modified thread object matching the specified ID.</returns>
        public async Task<Thread> ModifyThreadAsync(string threadId, Dictionary<string, object> metadata,
            CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(new { metadata = metadata }, OpenAIClient.JsonSerializationOptions)
                .ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}"), jsonContent, cancellationToken)
                .ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var thread = JsonSerializer.Deserialize<Thread>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return thread;
        }

        /// <summary>
        /// Delete a thread.
        /// </summary>
        /// <param name="threadId">The ID of the thread to delete.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>True, if was successfully deleted.</returns>
        public async Task<bool> DeleteThreadAsync(string threadId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.DeleteAsync(GetUrl($"/{threadId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var status =
                JsonSerializer.Deserialize<DeletionStatus>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return status.Deleted;
        }

        private sealed class DeletionStatus
        {
            [JsonInclude]
            [JsonPropertyName("id")]
            public string Id { get; private set; }

            [JsonInclude]
            [JsonPropertyName("object")]
            public string Object { get; private set; }

            [JsonInclude]
            [JsonPropertyName("deleted")]
            public bool Deleted { get; private set; }
        }
    }
}