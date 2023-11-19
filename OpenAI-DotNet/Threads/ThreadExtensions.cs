using OpenAI.Assistants;
using System.Collections.Generic;
using System.Linq;
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
            => await thread.Client.ThreadsEndpoint.RetrieveThreadAsync(thread, cancellationToken);

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
            => await thread.Client.ThreadsEndpoint.ModifyThreadAsync(thread, metadata, cancellationToken);

        /// <summary>
        /// Deletes the thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the thread was successfully deleted.</returns>
        public static async Task<bool> DeleteAsync(this ThreadResponse thread, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.DeleteThreadAsync(thread, cancellationToken);

        #region Messages

        /// <summary>
        /// Create a new message for this thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="request"><see cref="CreateMessageRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static async Task<MessageResponse> CreateMessageAsync(this ThreadResponse thread, CreateMessageRequest request, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.CreateMessageAsync(thread.Id, request, cancellationToken);

        /// <summary>
        /// List the messages associated to this thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{MessageResponse}"/></returns>
        public static async Task<ListResponse<MessageResponse>> ListMessagesAsync(this ThreadResponse thread, ListQuery query = null, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.ListMessagesAsync(thread.Id, query, cancellationToken);

        /// <summary>
        /// Retrieve a message.
        /// </summary>
        /// <param name="message"><see cref="MessageResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static async Task<MessageResponse> RetrieveAsync(this MessageResponse message, CancellationToken cancellationToken = default)
            => await message.Client.ThreadsEndpoint.RetrieveMessageAsync(message, cancellationToken);

        /// <summary>
        /// Retrieve a message.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="messageId">The id of the message to get.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static async Task<MessageResponse> RetrieveMessageAsync(this ThreadResponse thread, string messageId, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.RetrieveMessageAsync(thread.Id, messageId, cancellationToken);

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
            => await message.Client.ThreadsEndpoint.ModifyMessageAsync(message, metadata, cancellationToken);

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
            => await thread.Client.ThreadsEndpoint.ModifyMessageAsync(thread, messageId, metadata, cancellationToken);

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
            => await thread.Client.ThreadsEndpoint.ListFilesAsync(thread.Id, messageId, query, cancellationToken);

        /// <summary>
        /// Returns a list of message files.
        /// </summary>
        /// <param name="message"><see cref="MessageFileResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessageFile}"/>.</returns>
        public static async Task<ListResponse<MessageFileResponse>> ListFilesAsync(this MessageResponse message, ListQuery query = null, CancellationToken cancellationToken = default)
            => await message.Client.ThreadsEndpoint.ListFilesAsync(message, query, cancellationToken);

        /// <summary>
        /// Retrieve message file.
        /// </summary>
        /// <param name="message"><see cref="MessageResponse"/>.</param>
        /// <param name="fileId">The id of the file being retrieved.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageFileResponse"/>.</returns>
        public static async Task<MessageFileResponse> RetrieveFileAsync(this MessageResponse message, string fileId, CancellationToken cancellationToken = default)
            => await message.Client.ThreadsEndpoint.RetrieveFileAsync(message, fileId, cancellationToken);

        #endregion Files

        #region Runs

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="request"><see cref="CreateRunRequest"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CreateRunAsync(this ThreadResponse thread, CreateRunRequest request, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.CreateRunAsync(thread, request, cancellationToken);

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="assistantId">Id of the assistant to use for the run.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CreateRunAsync(this ThreadResponse thread, string assistantId, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.CreateRunAsync(thread, new CreateRunRequest(assistantId), cancellationToken);

        /// <summary>
        /// Create a thread and run it.
        /// </summary>
        /// <param name="assistant"><see cref="AssistantResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CreateThreadAndRunAsync(this AssistantResponse assistant, CancellationToken cancellationToken = default)
            => await assistant.Client.ThreadsEndpoint.CreateThreadAndRunAsync(new CreateThreadAndRunRequest(assistant.Id), cancellationToken);

        /// <summary>
        /// Gets the thread associated to the <see cref="RunResponse"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadResponse"/>.</returns>
        public static async Task<ThreadResponse> GetThreadAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.RetrieveThreadAsync(run.ThreadId, cancellationToken);

        /// <summary>
        /// List all of the runs associated to a thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{RunResponse}"/></returns>
        public static async Task<ListResponse<RunResponse>> ListRunsAsync(this ThreadResponse thread, ListQuery query = null, CancellationToken cancellationToken = default)
            => await thread.Client.ThreadsEndpoint.ListRunsAsync(thread.Id, query, cancellationToken);

        /// <summary>
        /// Get the latest status of the <see cref="RunResponse"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> UpdateAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.RetrieveRunAsync(run, cancellationToken);

        private static IEnumerable<RunStatus> DefaultStatusChecks { get; } = new[] { RunStatus.Queued, RunStatus.InProgress };

        /// <summary>
        /// Waits for <see cref="RunResponse.Status"/> to change from the provided <see cref="statusCheck"/>.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="statusCheck">Optional, <see cref="RunStatus"/> to wait for.<br/>Defaults to the current <see cref="RunResponse.Status"/>.</param>
        /// <param name="pollingInterval">Optional, time in milliseconds to wait before polling status.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> WaitForStatusChangeAsync(this RunResponse run, RunStatus? statusCheck = null, int? pollingInterval = null, CancellationToken cancellationToken = default)
            => await WaitForStatusChangeAsync(run, new[] { statusCheck ?? run.Status }, pollingInterval, cancellationToken);

        /// <summary>
        /// Waits for <see cref="RunResponse.Status"/> to longer be one of the provided <see cref="statusChecks"/>.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="statusChecks">Optional, collection of <see cref="RunStatus"/>s to wait for.<br/>Defaults to <see cref="RunStatus.Queued"/> and <see cref="RunStatus.InProgress"/>.</param>
        /// <param name="pollingInterval">Optional, time in milliseconds to wait before polling status.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> WaitForStatusChangeAsync(this RunResponse run, IEnumerable<RunStatus> statusChecks, int? pollingInterval = null, CancellationToken cancellationToken = default)
        {
            statusChecks ??= DefaultStatusChecks;
            var statuses = statusChecks?.ToList();
            pollingInterval ??= 500;
            RunResponse result;
            do
            {
                result = await run.UpdateAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                await Task.Delay(pollingInterval.Value, cancellationToken).ConfigureAwait(false);
            } while (statuses!.Contains(result.Status));
            return result;
        }

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
            => await run.Client.ThreadsEndpoint.ModifyRunAsync(run, metadata, cancellationToken);

        /// <summary>
        /// Cancels a run that is <see cref="RunStatus.InProgress"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static async Task<RunResponse> CancelAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => await run.Client.ThreadsEndpoint.CancelRunAsync(run, cancellationToken);

        #endregion Runs
    }
}