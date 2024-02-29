// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Threads
{
    public static class ThreadExtensions
    {
        /// <summary>
        /// Updates this thread with the latest snapshot from OpenAI.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadResponse"/>.</returns>
        public static async Task<ThreadResponse> UpdateAsync(this ThreadResponse thread, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.RetrieveThreadAsync(thread, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Modify the thread.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="ThreadResponse.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="metadata">The metadata to set on the thread.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadResponse"/>.</returns>
        public static async Task<ThreadResponse> ModifyAsync(this ThreadResponse thread, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.ModifyThreadAsync(thread, metadata, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Deletes the thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the thread was successfully deleted.</returns>
        public static async Task<bool> DeleteAsync(this ThreadResponse thread, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.DeleteThreadAsync(thread, cancellationToken).ConfigureAwait(false);

        #region Messages

        /// <summary>
        /// Create a new message for this thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="request"><see cref="CreateMessageRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static async Task<MessageResponse> CreateMessageAsync(this ThreadResponse thread, CreateMessageRequest request, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.CreateMessageAsync(thread.Id, request, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// List the messages associated to this thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{MessageResponse}"/></returns>
        public static async Task<ListResponse<MessageResponse>> ListMessagesAsync(this ThreadResponse thread, ListQuery query = null, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.ListMessagesAsync(thread.Id, query, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Retrieve a message.
        /// </summary>
        /// <param name="message"><see cref="MessageResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static async Task<MessageResponse> UpdateAsync(this MessageResponse message, CancellationToken cancellationToken = default)
            => await message.Client.ThreadsEndpoint.RetrieveMessageAsync(message.ThreadId, message.Id, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Retrieve a message.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="messageId">The id of the message to get.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static async Task<MessageResponse> RetrieveMessageAsync(this ThreadResponse thread, string messageId, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.RetrieveMessageAsync(thread.Id, messageId, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Modify a message.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="MessageResponse.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="message"><see cref="MessageResponse"/>.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static async Task<MessageResponse> ModifyAsync(this MessageResponse message, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => await message.Client.ThreadsEndpoint.ModifyMessageAsync(message, metadata, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Modifies a message.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="MessageResponse.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="messageId">The id of the message to modify.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static async Task<MessageResponse> ModifyMessageAsync(this ThreadResponse thread, string messageId, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.ModifyMessageAsync(thread, messageId, metadata, cancellationToken).ConfigureAwait(false);

        #endregion Messages

        #region Files

        /// <summary>
        /// Returns a list of message files.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="messageId">The id of the message that the files belongs to.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessageFile}"/>.</returns>
        public static async Task<ListResponse<MessageFileResponse>> ListFilesAsync(this ThreadResponse thread, string messageId, ListQuery query = null, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.ListFilesAsync(thread.Id, messageId, query, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns a list of message files.
        /// </summary>
        /// <param name="message"><see cref="MessageFileResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessageFile}"/>.</returns>
        public static async Task<ListResponse<MessageFileResponse>> ListFilesAsync(this MessageResponse message, ListQuery query = null, CancellationToken cancellationToken = default)
            => await message.Client.ThreadsEndpoint.ListFilesAsync(message.ThreadId, message.Id, query, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Retrieve message file.
        /// </summary>
        /// <param name="message"><see cref="MessageResponse"/>.</param>
        /// <param name="fileId">The id of the file being retrieved.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageFileResponse"/>.</returns>
        public static async Task<MessageFileResponse> RetrieveFileAsync(this MessageResponse message, string fileId, CancellationToken cancellationToken = default)
            => await message.Client.ThreadsEndpoint.RetrieveFileAsync(message.ThreadId, message.Id, fileId, cancellationToken).ConfigureAwait(false);

        // TODO 400 bad request errors. Likely OpenAI bug downloading message file content.
        ///// <summary>
        ///// Downloads a message file content to local disk.
        ///// </summary>
        ///// <param name="message"><see cref="MessageResponse"/>.</param>
        ///// <param name="fileId">The id of the file being retrieved.</param>
        ///// <param name="directory">Directory to save the file content.</param>
        ///// <param name="deleteCachedFile">Optional, delete cached file. Defaults to false.</param>
        ///// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        ///// <returns>Path to the downloaded file content.</returns>
        //public static async Task<string> DownloadFileContentAsync(this MessageResponse message, string fileId, string directory, bool deleteCachedFile = false, CancellationToken cancellationToken = default)
        //    => await message.Client.FilesEndpoint.DownloadFileAsync(fileId, directory, deleteCachedFile, cancellationToken).ConfigureAwait(false);

        // TODO 400 bad request errors. Likely OpenAI bug downloading message file content.
        ///// <summary>
        ///// Downloads a message file content to local disk.
        ///// </summary>
        ///// <param name="file"><see cref="MessageFileResponse"/>.</param>
        ///// <param name="directory">Directory to save the file content.</param>
        ///// <param name="deleteCachedFile">Optional, delete cached file. Defaults to false.</param>
        ///// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        ///// <returns>Path to the downloaded file content.</returns>
        //public static async Task<string> DownloadContentAsync(this MessageFileResponse file, string directory, bool deleteCachedFile = false, CancellationToken cancellationToken = default)
        //    => await file.Client.FilesEndpoint.DownloadFileAsync(file.Id, directory, deleteCachedFile, cancellationToken).ConfigureAwait(false);

        #endregion Files

        #region Runs

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="request"><see cref="CreateRunRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CreateRunAsync(this ThreadResponse thread, CreateRunRequest request = null, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.CreateRunAsync(thread, request, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="assistantId">Id of the assistant to use for the run.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CreateRunAsync(this ThreadResponse thread, string assistantId, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.CreateRunAsync(thread, new CreateRunRequest(assistantId), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Gets the thread associated to the <see cref="RunResponse"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadResponse"/>.</returns>
        public static async Task<ThreadResponse> GetThreadAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.RetrieveThreadAsync(run.ThreadId, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// List all of the runs associated to a thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{RunResponse}"/></returns>
        public static async Task<ListResponse<RunResponse>> ListRunsAsync(this ThreadResponse thread, ListQuery query = null, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.ListRunsAsync(thread.Id, query, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Get the latest status of the <see cref="RunResponse"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> UpdateAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.RetrieveRunAsync(run.ThreadId, run.Id, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Retrieves a run.
        /// </summary>
        /// <param name="thread">The thread that was run.</param>
        /// <param name="runId">The id of the run to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> RetrieveRunAsync(this ThreadResponse thread, string runId, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.RetrieveRunAsync(thread.Id, runId, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Modifies a run.
        /// </summary>
        /// <remarks>
        /// Only the <see cref="RunResponse.Metadata"/> can be modified.
        /// </remarks>
        /// <param name="run"><see cref="RunResponse"/> to modify.</param>
        /// <param name="metadata">Set of 16 key-value pairs that can be attached to an object.
        /// This can be useful for storing additional information about the object in a structured format.
        /// Keys can be a maximum of 64 characters long and values can be a maximum of 512 characters long.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> ModifyAsync(this RunResponse run, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.ModifyRunAsync(run.ThreadId, run.Id, metadata, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Waits for <see cref="RunResponse.Status"/> to change.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="pollingInterval">Optional, time in milliseconds to wait before polling status.</param>
        /// <param name="timeout">Optional, timeout in seconds to cancel polling.<br/>Defaults to 30 seconds.<br/>Set to -1 for indefinite.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> WaitForStatusChangeAsync(this RunResponse run, int? pollingInterval = null, int? timeout = null, CancellationToken cancellationToken = default)
        {
            using CancellationTokenSource cts = timeout.HasValue && timeout < 0
                ? new CancellationTokenSource()
                : new CancellationTokenSource(TimeSpan.FromSeconds(timeout ?? 30));
            using var chainedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
            RunResponse result;
            do
            {
                await Task.Delay(pollingInterval ?? 500, chainedCts.Token);
                cancellationToken.ThrowIfCancellationRequested();
                result = await run.UpdateAsync(cancellationToken: chainedCts.Token);
            } while (result.Status is RunStatus.Queued or RunStatus.InProgress or RunStatus.Cancelling);
            return result;
        }

        /// <summary>
        /// When a run has the status: "requires_action" and required_action.type is submit_tool_outputs,
        /// this endpoint can be used to submit the outputs from the tool calls once they're all completed.
        /// All outputs must be submitted in a single request.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to submit outputs for.</param>
        /// <param name="request"><see cref="SubmitToolOutputsRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> SubmitToolOutputsAsync(this RunResponse run, SubmitToolOutputsRequest request, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.SubmitToolOutputsAsync(run.ThreadId, run.Id, request, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// When a run has the status: "requires_action" and required_action.type is submit_tool_outputs,
        /// this endpoint can be used to submit the outputs from the tool calls once they're all completed.
        /// All outputs must be submitted in a single request.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to submit outputs for.</param>
        /// <param name="outputs"><see cref="ToolOutput"/>s</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> SubmitToolOutputsAsync(this RunResponse run, IEnumerable<ToolOutput> outputs, CancellationToken cancellationToken = default)
            => await run.SubmitToolOutputsAsync(new SubmitToolOutputsRequest(outputs), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns a list of run steps belonging to a run.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to list run steps for.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{RunStep}"/>.</returns>
        public static async Task<ListResponse<RunStepResponse>> ListRunStepsAsync(this RunResponse run, ListQuery query = null, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.ListRunStepsAsync(run.ThreadId, run.Id, query, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Retrieves a run step.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to retrieve step for.</param>
        /// <param name="runStepId">Id of the run step.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunStepResponse"/>.</returns>
        public static async Task<RunStepResponse> RetrieveRunStepAsync(this RunResponse run, string runStepId, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.RetrieveRunStepAsync(run.ThreadId, run.Id, runStepId, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Retrieves a run step.
        /// </summary>
        /// <param name="runStep"><see cref="RunStepResponse"/> to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunStepResponse"/>.</returns>
        public static async Task<RunStepResponse> UpdateAsync(this RunStepResponse runStep, CancellationToken cancellationToken = default)
            => await runStep.Client.ThreadsEndpoint.RetrieveRunStepAsync(runStep.ThreadId, runStep.RunId, runStep.Id, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Cancels a run that is <see cref="RunStatus.InProgress"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CancelAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.CancelRunAsync(run.ThreadId, run.Id, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Returns a list of messages for a given thread that the run belongs to.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessage}"/>.</returns>
        public static async Task<ListResponse<MessageResponse>> ListMessagesAsync(this RunResponse run, ListQuery query = null, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.ListMessagesAsync(run.ThreadId, query, cancellationToken).ConfigureAwait(false);

        #endregion Runs
    }
}