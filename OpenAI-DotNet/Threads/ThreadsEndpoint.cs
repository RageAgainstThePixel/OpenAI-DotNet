// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Threads
{
    /// <summary>
    /// Create threads that assistants can interact with.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/threads"/>
    /// </summary>
    public sealed class ThreadsEndpoint : OpenAIBaseEndpoint
    {
        public ThreadsEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "threads";

        /// <summary>
        /// Create a thread.
        /// </summary>
        /// <param name="request"><see cref="CreateThreadRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadResponse"/>.</returns>
        public async Task<ThreadResponse> CreateThreadAsync(CreateThreadRequest request = null, CancellationToken cancellationToken = default)
        {
            using var jsonContent = request != null ? JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent() : null;
            using var response = await client.Client.PostAsync(GetUrl(), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadResponse>(responseAsString, client);
        }

        /// <summary>
        /// Retrieves a thread.
        /// </summary>
        /// <param name="threadId">The id of the <see cref="ThreadResponse"/> to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadResponse"/>.</returns>
        public async Task<ThreadResponse> RetrieveThreadAsync(string threadId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadResponse>(responseAsString, client);
        }

        /// <summary>
        /// Modifies a thread.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="ThreadResponse.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="threadId">The id of the <see cref="ThreadResponse"/> to modify.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadResponse"/>.</returns>
        public async Task<ThreadResponse> ModifyThreadAsync(string threadId, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            using var jsonContent = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadResponse>(responseAsString, client);
        }

        /// <summary>
        /// Delete a thread.
        /// </summary>
        /// <param name="threadId">The id of the <see cref="ThreadResponse"/> to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if was successfully deleted.</returns>
        public async Task<bool> DeleteThreadAsync(string threadId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.DeleteAsync(GetUrl($"/{threadId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<DeletedResponse>(responseAsString, OpenAIClient.JsonSerializationOptions)?.Deleted ?? false;
        }

        #region Messages

        /// <summary>
        /// Create a message.
        /// </summary>
        /// <param name="threadId">The id of the thread to create a message for.</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public async Task<MessageResponse> CreateMessageAsync(string threadId, CreateMessageRequest request, CancellationToken cancellationToken = default)
        {
            using var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/messages"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<MessageResponse>(responseAsString, client);
        }

        /// <summary>
        /// Returns a list of messages for a given thread.
        /// </summary>
        /// <param name="threadId">The id of the thread the messages belong to.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessage}"/>.</returns>
        public async Task<ListResponse<MessageResponse>> ListMessagesAsync(string threadId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/messages", query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<MessageResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Retrieve a message.
        /// </summary>
        /// <param name="threadId">The id of the thread to which this message belongs.</param>
        /// <param name="messageId">The id of the message to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public async Task<MessageResponse> RetrieveMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/messages/{messageId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<MessageResponse>(responseAsString, client);
        }

        /// <summary>
        /// Modifies a message.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="MessageResponse.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="message"><see cref="MessageResponse"/> to modify.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public async Task<MessageResponse> ModifyMessageAsync(MessageResponse message, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => await ModifyMessageAsync(message.ThreadId, message.Id, metadata, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Modifies a message.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="MessageResponse.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="threadId">The id of the thread to which this message belongs.</param>
        /// <param name="messageId">The id of the message to modify.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public async Task<MessageResponse> ModifyMessageAsync(string threadId, string messageId, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            using var jsonContent = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/messages/{messageId}"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<MessageResponse>(responseAsString, client);
        }

        #endregion Messages

        #region Files

        /// <summary>
        /// Returns a list of message files.
        /// </summary>
        /// <param name="threadId">The id of the thread that the message and files belong to.</param>
        /// <param name="messageId">The id of the message that the files belongs to.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessageFile}"/>.</returns>
        public async Task<ListResponse<MessageFileResponse>> ListFilesAsync(string threadId, string messageId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/messages/{messageId}/files", query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<MessageFileResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Retrieve message file.
        /// </summary>
        /// <param name="threadId">The id of the thread to which the message and file belong.</param>
        /// <param name="messageId">The id of the message the file belongs to.</param>
        /// <param name="fileId">The id of the file being retrieved.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageFileResponse"/>.</returns>
        public async Task<MessageFileResponse> RetrieveFileAsync(string threadId, string messageId, string fileId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/messages/{messageId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<MessageFileResponse>(responseAsString, client);
        }

        #endregion Files

        #region Runs

        /// <summary>
        /// Returns a list of runs belonging to a thread.
        /// </summary>
        /// <param name="threadId">The id of the thread the run belongs to.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{RunResponse}"/></returns>
        public async Task<ListResponse<RunResponse>> ListRunsAsync(string threadId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/runs", query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<RunResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="threadId">The id of the thread to run.</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CreateRunAsync(string threadId, CreateRunRequest request = null, CancellationToken cancellationToken = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AssistantId))
            {
                var assistant = await client.AssistantsEndpoint.CreateAssistantAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                request = new CreateRunRequest(assistant, request);
            }

            using var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/runs"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunResponse>(responseAsString, client);
        }

        /// <summary>
        /// Create a thread and run it in one request.
        /// </summary>
        /// <param name="request"><see cref="CreateThreadAndRunRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CreateThreadAndRunAsync(CreateThreadAndRunRequest request = null, CancellationToken cancellationToken = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AssistantId))
            {
                var assistant = await client.AssistantsEndpoint.CreateAssistantAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                request = new CreateThreadAndRunRequest(assistant, request);
            }

            using var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl("/runs"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunResponse>(responseAsString, client);
        }

        /// <summary>
        /// Retrieves a run.
        /// </summary>
        /// <param name="threadId">The id of the thread that was run.</param>
        /// <param name="runId">The id of the run to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> RetrieveRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/runs/{runId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunResponse>(responseAsString, client);
        }

        /// <summary>
        /// Modifies a run.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="RunResponse.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="threadId">The id of the thread that was run.</param>
        /// <param name="runId">The id of the <see cref="RunResponse"/> to modify.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> ModifyRunAsync(string threadId, string runId, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            using var jsonContent = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunResponse>(responseAsString, client);
        }

        /// <summary>
        /// When a run has the status: "requires_action" and required_action.type is submit_tool_outputs,
        /// this endpoint can be used to submit the outputs from the tool calls once they're all completed.
        /// All outputs must be submitted in a single request.
        /// </summary>
        /// <param name="threadId">The id of the thread to which this run belongs.</param>
        /// <param name="runId">The id of the run that requires the tool output submission.</param>
        /// <param name="request"><see cref="SubmitToolOutputsRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> SubmitToolOutputsAsync(string threadId, string runId, SubmitToolOutputsRequest request, CancellationToken cancellationToken = default)
        {
            using var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}/submit_tool_outputs"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunResponse>(responseAsString, client);
        }

        /// <summary>
        /// Returns a list of run steps belonging to a run.
        /// </summary>
        /// <param name="threadId">The id of the thread to which the run and run step belongs.</param>
        /// <param name="runId">The id of the run to which the run step belongs.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{RunStep}"/>.</returns>
        public async Task<ListResponse<RunStepResponse>> ListRunStepsAsync(string threadId, string runId, ListQuery query = null, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/runs/{runId}/steps", query), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<RunStepResponse>>(responseAsString, client);
        }

        /// <summary>
        /// Retrieves a run step.
        /// </summary>
        /// <param name="threadId">The id of the thread to which the run and run step belongs.</param>
        /// <param name="runId">The id of the run to which the run step belongs.</param>
        /// <param name="stepId">The id of the run step to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunStepResponse"/>.</returns>
        public async Task<RunStepResponse> RetrieveRunStepAsync(string threadId, string runId, string stepId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/runs/{runId}/steps/{stepId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunStepResponse>(responseAsString, client);
        }

        /// <summary>
        /// Cancels a run that is <see cref="RunStatus.InProgress"/>.
        /// </summary>
        /// <param name="threadId">The id of the thread to which this run belongs.</param>
        /// <param name="runId">The id of the run to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CancelRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}/cancel"), content: null, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunResponse>(responseAsString, client);
        }

        #endregion Runs
    }
}