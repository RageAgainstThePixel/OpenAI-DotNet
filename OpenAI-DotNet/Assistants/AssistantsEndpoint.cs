// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Assistants
{
    public sealed class AssistantsEndpoint : OpenAIBaseEndpoint
    {
        internal AssistantsEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "assistants";

        /// <summary>
        /// Get list of assistants.
        /// </summary>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{AssistantResponse}"/>.</returns>
        public async Task<ListResponse<AssistantResponse>> ListAssistantsAsync(ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.GetAsync(GetUrl(queryParameters: query), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<ListResponse<AssistantResponse>>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create an assistant.
        /// </summary>
        /// <typeparam name="T"><see cref="JsonSchema"/> to use for structured outputs.</typeparam>
        /// <param name="request"><see cref="CreateAssistantRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public Task<AssistantResponse> CreateAssistantAsync<T>(CreateAssistantRequest request = null, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                request = new CreateAssistantRequest(jsonSchema: typeof(T));
            }
            else
            {
                request.ResponseFormatObject = new TextResponseFormatConfiguration(typeof(T));
            }

            return CreateAssistantAsync(request, cancellationToken);
        }

        /// <summary>
        /// Create an assistant.
        /// </summary>
        /// <param name="request"><see cref="CreateAssistantRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public async Task<AssistantResponse> CreateAssistantAsync(CreateAssistantRequest request = null, CancellationToken cancellationToken = default)
        {
            request ??= new CreateAssistantRequest();
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await HttpClient.PostAsync(GetUrl(), payload, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<AssistantResponse>(EnableDebug, payload, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves an assistant.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public async Task<AssistantResponse> RetrieveAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.GetAsync(GetUrl($"/{assistantId}"), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<AssistantResponse>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Modifies an assistant.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant to modify.</param>
        /// <param name="request"><see cref="CreateAssistantRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="AssistantResponse"/>.</returns>
        public async Task<AssistantResponse> ModifyAssistantAsync(string assistantId, CreateAssistantRequest request, CancellationToken cancellationToken = default)
        {
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await HttpClient.PostAsync(GetUrl($"/{assistantId}"), payload, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<AssistantResponse>(EnableDebug, payload, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete an assistant.
        /// </summary>
        /// <param name="assistantId">The ID of the assistant to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the assistant was deleted.</returns>
        public async Task<bool> DeleteAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
        {
            using var response = await HttpClient.DeleteAsync(GetUrl($"/{assistantId}"), cancellationToken).ConfigureAwait(false);
            var result = await response.DeserializeAsync<DeletedResponse>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
            return result?.Deleted ?? false;
        }
    }
}
