// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Realtime;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Extensions;

namespace OpenAI.Responses
{
    /// <summary>
    /// OpenAI's most advanced interface for generating model responses.
    /// Supports text and image inputs, and text outputs.
    /// Create stateful interactions with the model, using the output of previous responses as input.
    /// Extend the model's capabilities with built-in tools for file search, web search, computer use, and more.
    /// Allow the model access to external systems and data using function calling.
    /// </summary>
    public sealed class ResponsesEndpoint : OpenAIBaseEndpoint
    {
        public ResponsesEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "responses";

        /// <summary>
        /// Creates a model response.
        /// Provide text or image inputs to generate text or JSON outputs.
        /// Have the model call your own custom code or use built-in tools like web search or file search to use your own data as input for the model's response.
        /// </summary>
        /// <param name="request"><see cref="CreateResponseRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Response"/>.</returns>
        public async Task<Response> CreateModelResponseAsync(CreateResponseRequest request, CancellationToken cancellationToken = default)
        {
            var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl(), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<Response>(responseAsString, client);
        }

        /// <summary>
        /// Retrieves a model response with the given ID.
        /// </summary>
        /// <param name="responseId">The ID of the response to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <param name="include">
        /// Additional fields to include in the response.<br/>
        /// Currently supported values are:<br/>
        /// - file_search_call.results: Include the search results of the file search tool call.<br/>
        /// - message.input_image.image_url: Include image URLs from the computer call output.<br/>
        /// - computer_call_output.output.image_url: Include image urls from the computer call output.<br/>
        /// </param>
        /// <returns><see cref="Response"/>.</returns>
        public async Task<Response> GetModelResponseAsync(string responseId, CancellationToken cancellationToken = default, params string[] include)
        {
            var queryParameters = new Dictionary<string, string>();
            if (include is { Length: > 0 })
            {
                queryParameters.Add("include", string.Join(",", include));
            }
            var response = await client.Client.GetAsync(GetUrl($"/{responseId}", queryParameters), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken);
            return response.Deserialize<Response>(responseAsString, client);
        }

        /// <summary>
        /// Deletes a model response with the given ID.
        /// </summary>
        /// <param name="responseId">The ID of the response to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True if the response was deleted, false otherwise.</returns>
        public async Task<bool> DeleteModelResponseAsync(string responseId, CancellationToken cancellationToken = default)
        {
            var response = await client.Client.DeleteAsync(GetUrl($"/{responseId}"), cancellationToken);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken);
            var result = response.Deserialize<DeletedResponse>(responseAsString, client);
            return result.Deleted;
        }

        /// <summary>
        /// Cancels a model response with the given ID.
        /// </summary>
        /// <remarks>
        /// Only responses created with the background parameter set to true can be cancelled.
        /// </remarks>
        /// <param name="responseId">The ID of the response to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Response"/>.</returns>
        public async Task<Response> CancelModelResponsesAsync(string responseId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.PostAsync(GetUrl($"/{responseId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<Response>(responseAsString, client);
        }

        /// <summary>
        /// Returns a list of input items for a given response.
        /// </summary>
        /// <param name="responseId">The ID of the response to retrieve input items for.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{BaseResponse}"/>.</returns>
        public async Task<ListResponse<IResponseItem>> ListInputItemsAsync(string responseId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            var response = await client.Client.GetAsync(GetUrl($"/{responseId}/input_items", query), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<ListResponse<IResponseItem>>(EnableDebug, client, cancellationToken);
        }
    }
}
