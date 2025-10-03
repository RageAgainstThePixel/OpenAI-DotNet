// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Responses
{
    public sealed class ConversationsEndpoint : OpenAIBaseEndpoint
    {
        public ConversationsEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "conversations";

        /// <summary>
        /// Create a conversation.
        /// </summary>
        /// <param name="request"><see cref="CreateConversationRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Conversation"/>.</returns>
        public async Task<Conversation> CreateConversationAsync(CreateConversationRequest request, CancellationToken cancellationToken = default)
        {
            var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            var response = await PostAsync(GetUrl(), payload, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<Conversation>(EnableDebug, payload, client, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Get a conversation.
        /// </summary>
        /// <param name="conversationId">The id of the conversation to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Conversation"/>.</returns>
        public async Task<Conversation> GetConversationAsync(string conversationId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            var response = await GetAsync(GetUrl($"/{conversationId}"), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<Conversation>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Update a conversation.
        /// </summary>
        /// <param name="conversationId">
        /// The id of the conversation to retrieve.
        /// </param>
        /// <param name="metadata">
        /// Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format,
        /// and querying for objects via API or the dashboard.
        /// Keys are strings with a maximum length of 64 characters.Values are strings with a maximum length of 512 characters.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Conversation"/>.</returns>
        public async Task<Conversation> UpdateConversationAsync(string conversationId, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            var payload = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            var response = await PatchAsync(GetUrl($"/{conversationId}"), payload, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<Conversation>(EnableDebug, payload, client, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Delete a conversation.
        /// </summary>
        /// <remarks>
        /// Items in the conversation will not be deleted.
        /// </remarks>
        /// <param name="conversationId">The id of the conversation to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the <see cref="Conversation"/> was deleted successfully, otherwise False.</returns>
        public async Task<bool> DeleteConversationAsync(string conversationId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            var response = await DeleteAsync(GetUrl($"/{conversationId}"), cancellationToken).ConfigureAwait(false);
            var result = await response.DeserializeAsync<DeletedResponse>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
            return result.Deleted;
        }

        #region Conversation Items

        /// <summary>
        /// List all items for a conversation with the given ID.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation to list items for.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{IResponseItem}"/>.</returns>
        public async Task<ListResponse<IResponseItem>> ListConversationItemsAsync(string conversationId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            var response = await GetAsync(GetUrl($"/{conversationId}/items", query), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<ListResponse<IResponseItem>>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Create items in a conversation with the given ID.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation to add the item to.</param>
        /// <param name="items">The items to add to the conversation. You may add up to 20 items at a time.</param>
        /// <param name="include">Optional, Additional fields to include in the response.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{IResponseItem}"/>.</returns>
        public async Task<ListResponse<IResponseItem>> CreateConversationItemsAsync(string conversationId, IEnumerable<IResponseItem> items, string[] include = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var payload = JsonSerializer.Serialize(new { items }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            Dictionary<string, string> query = null;

            if (include is { Length: > 0 })
            {
                query = new Dictionary<string, string>
                {
                    { "include", string.Join(",", include) }
                };
            }

            var response = await PostAsync(GetUrl($"/{conversationId}/items", query), payload, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<ListResponse<IResponseItem>>(EnableDebug, payload, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve an item from a conversation.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation that contains the item.</param>
        /// <param name="itemId">The ID of the item to retrieve.</param>
        /// <param name="include">Optional, Additional fields to include in the response.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="IResponseItem"/>.</returns>
        public async Task<IResponseItem> GetConversationItemAsync(string conversationId, string itemId, string[] include = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            if (string.IsNullOrWhiteSpace(itemId))
            {
                throw new ArgumentNullException(nameof(itemId));
            }

            Dictionary<string, string> query = null;

            if (include is { Length: > 0 })
            {
                query = new Dictionary<string, string>
                {
                    { "include", string.Join(",", include) }
                };
            }

            var response = await GetAsync(GetUrl($"/{conversationId}/items/{itemId}", query), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<IResponseItem>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Delete an item from a conversation with the given IDs.
        /// </summary>
        /// <param name="conversationId">The ID of the conversation that contains the item.</param>
        /// <param name="itemId">The ID of the item to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>Returns the updated <see cref="Conversation"/>>.</returns>
        public async Task<Conversation> DeleteConversationItemAsync(string conversationId, string itemId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            if (string.IsNullOrWhiteSpace(itemId))
            {
                throw new ArgumentNullException(nameof(itemId));
            }

            var response = await DeleteAsync(GetUrl($"/{conversationId}/items/{itemId}"), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<Conversation>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }

        #endregion Conversation Items
    }
}
