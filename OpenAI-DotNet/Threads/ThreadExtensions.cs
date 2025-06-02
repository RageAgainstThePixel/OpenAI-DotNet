// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Assistants;
using System;
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
        public static Task<ThreadResponse> UpdateAsync(this ThreadResponse thread, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.RetrieveThreadAsync(thread, cancellationToken);

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
        public static Task<ThreadResponse> ModifyAsync(this ThreadResponse thread, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.ModifyThreadAsync(thread, metadata, cancellationToken);

        /// <summary>
        /// Deletes the thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="deleteToolResources">Optional, should tool resources, such as vector stores be deleted when this thread is deleted?</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True, if the thread was successfully deleted.</returns>
        public static async Task<bool> DeleteAsync(this ThreadResponse thread, bool deleteToolResources = false, CancellationToken cancellationToken = default)
        {
            if (deleteToolResources)
            {
                thread = await thread.UpdateAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var deleteTasks = new List<Task<bool>> { thread.Client.ThreadsEndpoint.DeleteThreadAsync(thread, cancellationToken) };

            if (deleteToolResources && thread.ToolResources?.FileSearch?.VectorStoreIds is { Count: > 0 })
            {
                deleteTasks.AddRange(
                    from vectorStoreId in thread.ToolResources?.FileSearch?.VectorStoreIds
                    where !string.IsNullOrWhiteSpace(vectorStoreId)
                    select thread.Client.VectorStoresEndpoint.DeleteVectorStoreAsync(vectorStoreId, cancellationToken));
            }

            await Task.WhenAll(deleteTasks).ConfigureAwait(false);
            return deleteTasks.TrueForAll(task => task.Result);
        }

        #region Messages

        /// <summary>
        /// Create a new message for this thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="request"><see cref="Message"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static Task<MessageResponse> CreateMessageAsync(this ThreadResponse thread, Message request, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.CreateMessageAsync(thread.Id, request, cancellationToken);

        /// <summary>
        /// List the messages associated to this thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{MessageResponse}"/>.</returns>
        public static Task<ListResponse<MessageResponse>> ListMessagesAsync(this ThreadResponse thread, ListQuery query = null, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.ListMessagesAsync(thread.Id, query, null, cancellationToken);

        /// <summary>
        /// Retrieve a message.
        /// </summary>
        /// <param name="message"><see cref="MessageResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static Task<MessageResponse> UpdateAsync(this MessageResponse message, CancellationToken cancellationToken = default)
            => message.Client.ThreadsEndpoint.RetrieveMessageAsync(message.ThreadId, message.Id, cancellationToken);

        /// <summary>
        /// Retrieve a message.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="messageId">The id of the message to get.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public static Task<MessageResponse> RetrieveMessageAsync(this ThreadResponse thread, string messageId, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.RetrieveMessageAsync(thread.Id, messageId, cancellationToken);

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
        public static Task<MessageResponse> ModifyAsync(this MessageResponse message, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => message.Client.ThreadsEndpoint.ModifyMessageAsync(message, metadata, cancellationToken);

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
        public static Task<MessageResponse> ModifyMessageAsync(this ThreadResponse thread, string messageId, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.ModifyMessageAsync(thread, messageId, metadata, cancellationToken);

        #endregion Messages

        #region Runs

        [Obsolete("use new overload with Func<IServerSentEvent, Task> instead.")]
        public static Task<RunResponse> CreateRunAsync(this ThreadResponse thread, CreateRunRequest request, Action<IServerSentEvent> streamEventHandler, CancellationToken cancellationToken = default)
            => thread.CreateRunAsync(request, streamEvent =>
            {
                streamEventHandler?.Invoke(streamEvent);
                return Task.CompletedTask;
            }, cancellationToken);

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="request"><see cref="CreateRunRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static Task<RunResponse> CreateRunAsync(this ThreadResponse thread, CreateRunRequest request = null, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.CreateRunAsync(thread, request, streamEventHandler, cancellationToken);

        [Obsolete("use new overload with Func<IServerSentEvent, Task> instead.")]
        public static Task<RunResponse> CreateRunAsync(this ThreadResponse thread, AssistantResponse assistant, Action<IServerSentEvent> streamEventHandler, CancellationToken cancellationToken = default)
            => thread.CreateRunAsync(assistant, streamEvent =>
            {
                streamEventHandler?.Invoke(streamEvent);
                return Task.CompletedTask;
            }, cancellationToken);

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="assistant">The <see cref="AssistantResponse"/> to use for the run.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static Task<RunResponse> CreateRunAsync(this ThreadResponse thread, AssistantResponse assistant, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
        {
            var request = new CreateRunRequest(
                assistant,
                model: assistant.Model,
                instructions: assistant.Instructions,
                tools: assistant.Tools,
                temperature: assistant.ReasoningEffort > 0 ? null : assistant.Temperature,
                topP: assistant.ReasoningEffort > 0 ? null : assistant.TopP,
                reasoningEffort: assistant.ReasoningEffort,
                responseFormat: assistant.ResponseFormat);
            return thread.Client.ThreadsEndpoint.CreateRunAsync(thread, request, streamEventHandler, cancellationToken);
        }

        /// <summary>
        /// Gets the thread associated to the <see cref="RunResponse"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ThreadResponse"/>.</returns>
        public static Task<ThreadResponse> GetThreadAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => run.Client.ThreadsEndpoint.RetrieveThreadAsync(run.ThreadId, cancellationToken);

        /// <summary>
        /// Lists all the runs associated to a thread.
        /// </summary>
        /// <param name="thread"><see cref="ThreadResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{RunResponse}"/>.</returns>
        public static Task<ListResponse<RunResponse>> ListRunsAsync(this ThreadResponse thread, ListQuery query = null, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.ListRunsAsync(thread.Id, query, cancellationToken);

        /// <summary>
        /// Get the latest status of the <see cref="RunResponse"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static Task<RunResponse> UpdateAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => run.Client.ThreadsEndpoint.RetrieveRunAsync(run.ThreadId, run.Id, cancellationToken);

        /// <summary>
        /// Retrieves a run.
        /// </summary>
        /// <param name="thread">The thread that was run.</param>
        /// <param name="runId">The id of the run to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static Task<RunResponse> RetrieveRunAsync(this ThreadResponse thread, string runId, CancellationToken cancellationToken = default)
            => thread.Client.ThreadsEndpoint.RetrieveRunAsync(thread.Id, runId, cancellationToken);

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
        public static Task<RunResponse> ModifyAsync(this RunResponse run, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => run.Client.ThreadsEndpoint.ModifyRunAsync(run.ThreadId, run.Id, metadata, cancellationToken);

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
            if (run.Status is not (RunStatus.Queued or RunStatus.InProgress or RunStatus.Cancelling))
            {
                return run;
            }
            using CancellationTokenSource cts = timeout is < 0
                ? new CancellationTokenSource()
                : new CancellationTokenSource(TimeSpan.FromSeconds(timeout ?? 30));
            using var chainedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
            RunResponse result;
            do
            {
                await Task.Delay(pollingInterval ?? 500, chainedCts.Token).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                result = await run.UpdateAsync(cancellationToken: chainedCts.Token).ConfigureAwait(false);
            } while (result.Status is RunStatus.Queued or RunStatus.InProgress or RunStatus.Cancelling);
            return result;
        }

        [Obsolete("use new overload with Func<IServerSentEvent, Task> instead.")]
        public static Task<RunResponse> SubmitToolOutputsAsync(this RunResponse run, SubmitToolOutputsRequest request, Action<IServerSentEvent> streamEventHandler, CancellationToken cancellationToken = default)
            => run.SubmitToolOutputsAsync(request, streamEvent =>
            {
                streamEventHandler?.Invoke(streamEvent);
                return Task.CompletedTask;
            }, cancellationToken);

        /// <summary>
        /// When a run has the status: "requires_action" and required_action.type is submit_tool_outputs,
        /// this endpoint can be used to submit the outputs from the tool calls once they're all completed.
        /// All outputs must be submitted in a single request.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to submit outputs for.</param>
        /// <param name="request"><see cref="SubmitToolOutputsRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static Task<RunResponse> SubmitToolOutputsAsync(this RunResponse run, SubmitToolOutputsRequest request, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
            => run.Client.ThreadsEndpoint.SubmitToolOutputsAsync(run.ThreadId, run.Id, request, streamEventHandler, cancellationToken);

        [Obsolete("use new overload with Func<IServerSentEvent, Task> instead.")]
        public static Task<RunResponse> SubmitToolOutputsAsync(this RunResponse run, IEnumerable<ToolOutput> outputs, Action<IServerSentEvent> streamEventHandler, CancellationToken cancellationToken = default)
            => run.SubmitToolOutputsAsync(new SubmitToolOutputsRequest(outputs), streamEvent =>
            {
                streamEventHandler?.Invoke(streamEvent);
                return Task.CompletedTask;
            }, cancellationToken);

        /// <summary>
        /// When a run has the status: "requires_action" and required_action.type is submit_tool_outputs,
        /// this endpoint can be used to submit the outputs from the tool calls once they're all completed.
        /// All outputs must be submitted in a single request.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to submit outputs for.</param>
        /// <param name="outputs"><see cref="ToolOutput"/>s</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static Task<RunResponse> SubmitToolOutputsAsync(this RunResponse run, IEnumerable<ToolOutput> outputs, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
            => run.SubmitToolOutputsAsync(new SubmitToolOutputsRequest(outputs), streamEventHandler, cancellationToken);

        /// <summary>
        /// Returns a list of run steps belonging to a run.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to list run steps for.</param>
        /// <param name="query">Optional, <see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{RunStep}"/>.</returns>
        public static Task<ListResponse<RunStepResponse>> ListRunStepsAsync(this RunResponse run, ListQuery query = null, CancellationToken cancellationToken = default)
            => run.Client.ThreadsEndpoint.ListRunStepsAsync(run.ThreadId, run.Id, query, cancellationToken);

        /// <summary>
        /// Retrieves a run step.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to retrieve step for.</param>
        /// <param name="runStepId">Id of the run step.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunStepResponse"/>.</returns>
        public static Task<RunStepResponse> RetrieveRunStepAsync(this RunResponse run, string runStepId, CancellationToken cancellationToken = default)
            => run.Client.ThreadsEndpoint.RetrieveRunStepAsync(run.ThreadId, run.Id, runStepId, cancellationToken);

        /// <summary>
        /// Retrieves a run step.
        /// </summary>
        /// <param name="runStep"><see cref="RunStepResponse"/> to retrieve.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunStepResponse"/>.</returns>
        public static Task<RunStepResponse> UpdateAsync(this RunStepResponse runStep, CancellationToken cancellationToken = default)
            => runStep.Client.ThreadsEndpoint.RetrieveRunStepAsync(runStep.ThreadId, runStep.RunId, runStep.Id, cancellationToken);

        /// <summary>
        /// Cancels a run that is <see cref="RunStatus.InProgress"/>.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/> to cancel.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public static Task<bool> CancelAsync(this RunResponse run, CancellationToken cancellationToken = default)
            => run.Client.ThreadsEndpoint.CancelRunAsync(run.ThreadId, run.Id, cancellationToken);

        /// <summary>
        /// Returns a list of messages for a given thread that the run belongs to.
        /// </summary>
        /// <param name="run"><see cref="RunResponse"/>.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessage}"/>.</returns>
        public static Task<ListResponse<MessageResponse>> ListMessagesAsync(this RunResponse run, ListQuery query = null, CancellationToken cancellationToken = default)
            => run.Client.ThreadsEndpoint.ListMessagesAsync(run.ThreadId, query, run.Id, cancellationToken);

        #endregion Runs
    }
}
