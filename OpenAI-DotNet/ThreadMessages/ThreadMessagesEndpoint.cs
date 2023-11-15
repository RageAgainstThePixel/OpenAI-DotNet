using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OpenAI.Extensions;

namespace OpenAI.ThreadMessages
{
    public class ThreadMessagesEndpoint : BaseEndPoint
    {
        public ThreadMessagesEndpoint(OpenAIClient api) : base(api) { }

        protected override string Root => "threads";
    
        /// <summary>
        /// Create a message.
        /// </summary>
        /// <param name="threadId">The id of the thread to create a message for.</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadMessage"/>.</returns>
        public async Task<ThreadMessage> CreateThreadMessageAsync(string threadId, CreateThreadMessageRequest request, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/messages"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var created = JsonSerializer.Deserialize<ThreadMessage>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return created;
        }

        /// <summary>
        /// Retrieve a message.
        /// </summary>
        /// <param name="threadId">The id of the thread to which this message belongs.</param>
        /// <param name="messageId">The id of the message to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>The message object matching the specified id.</returns>
        public async Task<ThreadMessage> RetrieveThreadMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/messages/{messageId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var created = JsonSerializer.Deserialize<ThreadMessage>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return created;
        }

        /// <summary>
        /// Modifies a message.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="ThreadMessage.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="threadId">The id of the thread to which this message belongs.</param>
        /// <param name="messageId">The id of the message to modify.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadMessage"/>.</returns>
        public async Task<ThreadMessage> ModifyThreadMessageAsync(
            string threadId, string messageId, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(new { metadata = metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/messages/{messageId}"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var created = JsonSerializer.Deserialize<ThreadMessage>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return created;
        }

        /// <summary>
        /// Returns a list of messages for a given thread.
        /// </summary>
        /// <param name="threadId">The id of the thread the messages belong to.</param>
        /// <param name="limit">A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20.</param>
        /// <param name="order">Sort order by the created_at timestamp of the objects. asc for ascending order and desc for descending order.</param>
        /// <param name="after">A cursor for use in pagination. after is an object id that defines your place in the list.
        /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
        /// your subsequent call can include after=obj_foo in order to fetch the next page of the list.
        /// </param>
        /// <param name="before">A cursor for use in pagination. before is an object id that defines your place in the list.
        /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
        /// your subsequent call can include before=obj_foo in order to fetch the previous page of the list.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadMessagesList"/>.</returns>
        public async Task<ThreadMessagesList> ListThreadMessagesAsync(
            string threadId, int? limit = null, string order = "desc", string after = null, string before = null,
            CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>();
            if (limit.HasValue) parameters.Add("limit", limit.ToString());
            if (!String.IsNullOrEmpty(order)) parameters.Add("order", order);
            if (!String.IsNullOrEmpty(after)) parameters.Add("after", after);
            if (!String.IsNullOrEmpty(before)) parameters.Add("before", before);

            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/messages", parameters), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var messages = JsonSerializer.Deserialize<ThreadMessagesList>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return messages;
        }

        /// <summary>
        /// Retrieve message file
        /// </summary>
        /// <param name="threadId">The id of the thread to which the message and File belong.</param>
        /// <param name="messageId">The id of the message the file belongs to.</param>
        /// <param name="fileId">The id of the file being retrieved.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadMessageFile"/>.</returns>
        public async Task<ThreadMessageFile> RetrieveMessageFile(
            string threadId, string messageId, string fileId,
            CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/messages/{messageId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var created = JsonSerializer.Deserialize<ThreadMessageFile>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return created;
        }
    
        /// <summary>
        /// Returns a list of message files.
        /// </summary>
        /// <param name="threadId">The id of the thread that the message and files belong to.</param>
        /// <param name="messageId">The id of the message that the files belongs to.</param>
        /// <param name="limit">A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20.</param>
        /// <param name="order">Sort order by the created_at timestamp of the objects. asc for ascending order and desc for descending order.</param>
        /// <param name="after">A cursor for use in pagination. after is an object id that defines your place in the list.
        /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
        /// your subsequent call can include after=obj_foo in order to fetch the next page of the list.
        /// </param>
        /// <param name="before">A cursor for use in pagination. before is an object id that defines your place in the list.
        /// For instance, if you make a list request and receive 100 objects, ending with obj_foo,
        /// your subsequent call can include before=obj_foo in order to fetch the previous page of the list.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadMessageFilesList"/>.</returns>
        public async Task<ThreadMessageFilesList> ListMessageFilesAsync(
            string threadId, string messageId, int? limit = null, string order = "desc", string after = null, string before = null,
            CancellationToken cancellationToken = default)
        {
            var parameters = new Dictionary<string, string>();
            if (limit.HasValue) parameters.Add("limit", limit.ToString());
            if (!String.IsNullOrEmpty(order)) parameters.Add("order", order);
            if (!String.IsNullOrEmpty(after)) parameters.Add("after", after);
            if (!String.IsNullOrEmpty(before)) parameters.Add("before", before);

            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/messages/{messageId}/files", parameters), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            var messages = JsonSerializer.Deserialize<ThreadMessageFilesList>(responseAsString, OpenAIClient.JsonSerializationOptions);

            return messages;
        }
    }
}