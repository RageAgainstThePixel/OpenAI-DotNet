using OpenAI.Extensions;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Threads
{
    /// <summary>
    /// Create threads that assistants can interact with.
    /// <see href="https://platform.openai.com/docs/api-reference/threads"/>
    /// </summary>
    public class ThreadsEndpoint : BaseEndPoint
    {
        public ThreadsEndpoint(OpenAIClient api) : base(api) { }

        protected override string Root => "threads";

        /// <summary>
        /// Create a thread.
        /// </summary>
        /// <param name="request"><see cref="CreateThreadRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Thread"/>.</returns>
        public async Task<Thread> CreateThreadAsync(CreateThreadRequest request, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl(), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<Thread>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Retrieves a thread.
        /// </summary>
        /// <param name="threadId">The id of the <see cref="Thread"/> to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Thread"/>.</returns>
        public async Task<Thread> RetrieveThreadAsync(string threadId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<Thread>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Modifies a thread.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="Thread.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="threadId">The id of the <see cref="Thread"/> to modify.</param>
        /// <param name="metaData">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Thread"/>.</returns>
        public async Task<Thread> ModifyThreadAsync(string threadId, IReadOnlyDictionary<string, string> metaData, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(new { metaData }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<Thread>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Delete a thread.
        /// </summary>
        /// <param name="threadId">The id of the <see cref="Thread"/> to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if was successfully deleted.</returns>
        public async Task<bool> DeleteThreadAsync(string threadId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.DeleteAsync(GetUrl($"/{threadId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return JsonSerializer.Deserialize<DeletedResponse>(responseAsString, OpenAIClient.JsonSerializationOptions)?.Deleted ?? false;
        }

        #region Messages

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
            return response.Deserialize<ThreadMessage>(responseAsString, OpenAIClient.JsonSerializationOptions);
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
            return response.Deserialize<ThreadMessage>(responseAsString, OpenAIClient.JsonSerializationOptions);
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
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadMessage"/>.</returns>
        public async Task<ThreadMessage> ModifyThreadMessageAsync(string threadId, string messageId, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/messages/{messageId}"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadMessage>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Returns a list of messages for a given thread.
        /// </summary>
        /// <param name="threadId">The id of the thread the messages belong to.</param>
        /// <param name="request"><see cref="ListRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessage}"/>.</returns>
        public async Task<ListResponse<ThreadMessage>> ListThreadMessagesAsync(string threadId, ListRequest request = null, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/messages", request), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<ThreadMessage>>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        #endregion Messages

        #region Files

        /// <summary>
        /// Retrieve message file
        /// </summary>
        /// <param name="threadId">The id of the thread to which the message and File belong.</param>
        /// <param name="messageId">The id of the message the file belongs to.</param>
        /// <param name="fileId">The id of the file being retrieved.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadMessageFile"/>.</returns>
        public async Task<ThreadMessageFile> RetrieveMessageFile(string threadId, string messageId, string fileId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/messages/{messageId}/files/{fileId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadMessageFile>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Returns a list of message files.
        /// </summary>
        /// <param name="threadId">The id of the thread that the message and files belong to.</param>
        /// <param name="messageId">The id of the message that the files belongs to.</param>
        /// <param name="request"><see cref="ListRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessageFile}"/>.</returns>
        public async Task<ListResponse<ThreadMessageFile>> ListMessageFilesAsync(string threadId, string messageId, ListRequest request = null, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/messages/{messageId}/files", request), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<ThreadMessageFile>>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        #endregion Files

        #region Runs

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="threadId">The id of the thread to run.</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadRun"/>.</returns>
        public async Task<ThreadRun> CreateThreadRunAsync(string threadId, CreateThreadRunRequest request, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/runs"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Retrieves a run.
        /// </summary>
        /// <param name="threadId">The id of the thread that was run.</param>
        /// <param name="runId">The id of the run to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadRun"/>.</returns>
        public async Task<ThreadRun> RetrieveRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/runs/{runId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Modifies a run.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="ThreadRun.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="threadId">The id of the thread that was run.</param>
        /// <param name="runId">The id of the <see cref="ThreadRun"/> to modify.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadRun"/>.</returns>
        public async Task<ThreadRun> ModifyThreadRunAsync(string threadId, string runId, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Returns a list of runs belonging to a thread.
        /// </summary>
        /// <param name="threadId">The id of the thread the run belongs to.</param>
        /// <param name="request"><see cref="ListRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>A list of run objects.</returns>
        public async Task<ListResponse<ThreadRun>> ListThreadRunsAsync(string threadId, ListRequest request = null, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/runs", request), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<ThreadRun>>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// When a run has the status: "requires_action" and required_action.type is submit_tool_outputs,
        /// this endpoint can be used to submit the outputs from the tool calls once they're all completed.
        /// All outputs must be submitted in a single request.
        /// </summary>
        /// <param name="threadId">The id of the thread to which this run belongs.</param>
        /// <param name="runId">The id of the run that requires the tool output submission.</param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadRun"/>.</returns>
        public async Task<ThreadRun> SubmitToolOutputsAsync(string threadId, string runId, SubmitThreadRunToolOutputsRequest request, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}/submit_tool_outputs"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Cancels a run that is in_progress.
        /// </summary>
        /// <param name="threadId">The id of the thread to which this run belongs.</param>
        /// <param name="runId">The id of the run to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadRun"/>.</returns>
        public async Task<ThreadRun> CancelThreadRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}/cancel"), content: null, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Create a thread and run it in one request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadRun"/>.</returns>
        public async Task<ThreadRun> CreateThreadAndRunAsync(CreateThreadAndRunRequest request, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent(EnableDebug);
            var response = await Api.Client.PostAsync(GetUrl($"/runs"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ThreadRun>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Retrieves a run step.
        /// </summary>
        /// <param name="threadId">The id of the thread to which the run and run step belongs.</param>
        /// <param name="runId">The id of the run to which the run step belongs.</param>
        /// <param name="stepId">The id of the run step to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunStep"/>.</returns>
        public async Task<RunStep> RetrieveTheadRunStepAsync(string threadId, string runId, string stepId, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/runs/{runId}/steps/{stepId}"), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunStep>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        /// <summary>
        /// Retrieves a run step.
        /// </summary>
        /// <param name="threadId">The id of the thread to which the run and run step belongs.</param>
        /// <param name="runId">The id of the run to which the run step belongs.</param>
        /// <param name="request"><see cref="ListRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{RunStep}"/>.</returns>
        public async Task<ListResponse<RunStep>> ListTheadRunStepsAsync(string threadId, string runId, ListRequest request = null, CancellationToken cancellationToken = default)
        {
            var response = await Api.Client.GetAsync(GetUrl($"/{threadId}/runs/{runId}/steps", request), cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ListResponse<RunStep>>(responseAsString, OpenAIClient.JsonSerializationOptions);
        }

        #endregion Runs
    }
}