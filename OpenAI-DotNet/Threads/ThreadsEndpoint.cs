// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            using var payload = request != null ? JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent() : null;
            using var response = await client.Client.PostAsync(GetUrl(), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
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
            using var payload = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
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
            return response.Deserialize<DeletedResponse>(responseAsString, client)?.Deleted ?? false;
        }

        #region Messages

        /// <summary>
        /// Create a message.
        /// </summary>
        /// <param name="threadId">The id of the thread to create a message for.</param>
        /// <param name="message"><see cref="Message"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="MessageResponse"/>.</returns>
        public async Task<MessageResponse> CreateMessageAsync(string threadId, Message message, CancellationToken cancellationToken = default)
        {
            using var payload = JsonSerializer.Serialize(message, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/messages"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<MessageResponse>(responseAsString, client);
        }

        /// <summary>
        /// Returns a list of messages for a given thread.
        /// </summary>
        /// <param name="threadId">The id of the thread the messages belong to.</param>
        /// <param name="query"><see cref="ListQuery"/>.</param>
        /// <param name="runId">Optional, filter messages by the run ID that generated them.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ListResponse{ThreadMessage}"/>.</returns>
        public async Task<ListResponse<MessageResponse>> ListMessagesAsync(string threadId, ListQuery query = null, string runId = null, CancellationToken cancellationToken = default)
        {
            Dictionary<string, string> queryParams = query;

            if (!string.IsNullOrWhiteSpace(runId))
            {
                queryParams ??= new();
                queryParams.Add("run_id", runId);
            }

            using var response = await client.Client.GetAsync(GetUrl($"/{threadId}/messages", queryParams), cancellationToken).ConfigureAwait(false);
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
            using var payload = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/messages/{messageId}"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<MessageResponse>(responseAsString, client);
        }

        #endregion Messages

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
        /// <param name="request"><see cref="CreateRunRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CreateRunAsync(string threadId, CreateRunRequest request = null, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
            => await CreateRunAsync(threadId, request, streamEventHandler == null ? null : async (_, serverSentEvent) =>
            {
                await streamEventHandler.Invoke(serverSentEvent);
            }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <typeparam name="T"><see cref="JsonSchema"/> to use for structured outputs.</typeparam>
        /// <param name="threadId">The id of the thread to run.</param>
        /// <param name="request"><see cref="CreateRunRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CreateRunAsync<T>(string threadId, CreateRunRequest request = null, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request?.AssistantId))
            {
                var assistant = await client.AssistantsEndpoint.CreateAssistantAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (request == null)
                {
                    request = new CreateRunRequest(assistant);
                }
                else
                {
                    request.AssistantId = assistant.Id;
                }
            }

            request.ResponseFormatObject = new ResponseFormatObject(typeof(T));
            return await CreateRunAsync(threadId, request, streamEventHandler == null ? null : async (_, serverSentEvent) =>
            {
                await streamEventHandler.Invoke(serverSentEvent);
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a run.
        /// </summary>
        /// <param name="threadId">The id of the thread to run.</param>
        /// <param name="request"><see cref="CreateRunRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{String, IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CreateRunAsync(string threadId, CreateRunRequest request = null, Func<string, IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request?.AssistantId))
            {
                var assistant = await client.AssistantsEndpoint.CreateAssistantAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (request == null)
                {
                    request = new CreateRunRequest(assistant);
                }
                else
                {
                    request.AssistantId = assistant.Id;
                }
            }

            request.Stream = streamEventHandler != null;
            var endpoint = GetUrl($"/{threadId}/runs");
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();

            if (request.Stream)
            {
                return await StreamRunAsync(endpoint, payload, streamEventHandler, cancellationToken).ConfigureAwait(false);
            }

            using var response = await client.Client.PostAsync(endpoint, payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<RunResponse>(responseAsString, client);
        }

        /// <summary>
        /// Create a thread and run it in one request.
        /// </summary>
        /// <param name="request"><see cref="CreateThreadAndRunRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CreateThreadAndRunAsync(CreateThreadAndRunRequest request = null, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
            => await CreateThreadAndRunAsync(request, streamEventHandler == null ? null : async (_, serverSentEvent) =>
            {
                await streamEventHandler.Invoke(serverSentEvent);
            }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Create a thread and run it in one request.
        /// </summary>
        /// <typeparam name="T"><see cref="JsonSchema"/> to use for structured outputs.</typeparam>
        /// <param name="request"><see cref="CreateThreadAndRunRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CreateThreadAndRunAsync<T>(CreateThreadAndRunRequest request = null, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request?.AssistantId))
            {
                var assistant = await client.AssistantsEndpoint.CreateAssistantAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (request == null)
                {
                    request = new CreateThreadAndRunRequest(assistant, jsonSchema: typeof(T));
                }
                else
                {
                    request.AssistantId = assistant.Id;
                }
            }

            request.ResponseFormatObject = new ResponseFormatObject(typeof(T));
            return await CreateThreadAndRunAsync(request, streamEventHandler == null ? null : async (_, serverSentEvent) =>
            {
                await streamEventHandler.Invoke(serverSentEvent).ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a thread and run it in one request.
        /// </summary>
        /// <param name="request"><see cref="CreateThreadAndRunRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{String, IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> CreateThreadAndRunAsync(CreateThreadAndRunRequest request = null, Func<string, IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request?.AssistantId))
            {
                var assistant = await client.AssistantsEndpoint.CreateAssistantAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

                if (request == null)
                {
                    request = new CreateThreadAndRunRequest(assistant);
                }
                else
                {
                    request.AssistantId = assistant.Id;
                }
            }

            request.Stream = streamEventHandler != null;
            var endpoint = GetUrl("/runs");
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();

            if (request.Stream)
            {
                return await StreamRunAsync(endpoint, payload, streamEventHandler, cancellationToken).ConfigureAwait(false);
            }

            using var response = await client.Client.PostAsync(endpoint, payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
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
            using var payload = JsonSerializer.Serialize(new { metadata }, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
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
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> SubmitToolOutputsAsync(string threadId, string runId, SubmitToolOutputsRequest request, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
            => await SubmitToolOutputsAsync(threadId, runId, request, streamEventHandler == null ? null : async (_, serverSentEvent) =>
            {
                await streamEventHandler.Invoke(serverSentEvent);
            }, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// When a run has the status: "requires_action" and required_action.type is submit_tool_outputs,
        /// this endpoint can be used to submit the outputs from the tool calls once they're all completed.
        /// All outputs must be submitted in a single request.
        /// </summary>
        /// <param name="threadId">The id of the thread to which this run belongs.</param>
        /// <param name="runId">The id of the run that requires the tool output submission.</param>
        /// <param name="request"><see cref="SubmitToolOutputsRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{String, IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="RunResponse"/>.</returns>
        public async Task<RunResponse> SubmitToolOutputsAsync(string threadId, string runId, SubmitToolOutputsRequest request, Func<string, IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
        {
            request.Stream = streamEventHandler != null;
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            var endpoint = GetUrl($"/{threadId}/runs/{runId}/submit_tool_outputs");

            if (request.Stream)
            {
                return await StreamRunAsync(endpoint, payload, streamEventHandler, cancellationToken).ConfigureAwait(false);
            }

            using var response = await client.Client.PostAsync(endpoint, payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
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
        public async Task<bool> CancelRunAsync(string threadId, string runId, CancellationToken cancellationToken = default)
        {
            using var response = await client.Client.PostAsync(GetUrl($"/{threadId}/runs/{runId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, cancellationToken: cancellationToken).ConfigureAwait(false);
            var run = response.Deserialize<RunResponse>(responseAsString, client);

            if (run.Status < RunStatus.Cancelling)
            {
                try
                {
                    run = await run.WaitForStatusChangeAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return run.Status >= RunStatus.Cancelling;
        }

        #endregion Runs

        private async Task<RunResponse> StreamRunAsync(string endpoint, StringContent payload, Func<string, IServerSentEvent, Task> streamEventHandler, CancellationToken cancellationToken = default)
        {
            RunResponse run = null;
            RunStepResponse runStep = null;
            MessageResponse message = null;

            using var response = await this.StreamEventsAsync(endpoint, payload, async (sseResponse, ssEvent) =>
            {
                IServerSentEvent serverSentEvent = default;
                var @event = ssEvent.Value.GetValue<string>();

                try
                {
                    switch (@event)
                    {
                        case "thread.created":
                            serverSentEvent = sseResponse.Deserialize<ThreadResponse>(ssEvent, client);
                            break;
                        case "thread.run.created":
                        case "thread.run.queued":
                        case "thread.run.in_progress":
                        case "thread.run.requires_action":
                        case "thread.run.completed":
                        case "thread.run.incomplete":
                        case "thread.run.failed":
                        case "thread.run.cancelling":
                        case "thread.run.cancelled":
                        case "thread.run.expired":
                            var partialRun = sseResponse.Deserialize<RunResponse>(ssEvent, client);

                            if (run == null)
                            {
                                run = partialRun;
                            }
                            else
                            {
                                run.AppendFrom(partialRun);
                            }

                            serverSentEvent = run;
                            break;
                        case "thread.run.step.created":
                        case "thread.run.step.in_progress":
                        case "thread.run.step.delta":
                        case "thread.run.step.completed":
                        case "thread.run.step.failed":
                        case "thread.run.step.cancelled":
                        case "thread.run.step.expired":
                            var partialRunStep = sseResponse.Deserialize<RunStepResponse>(ssEvent, client);

                            if (runStep == null ||
                                runStep.Id != partialRunStep.Id)
                            {
                                runStep = partialRunStep;
                            }
                            else
                            {
                                runStep.AppendFrom(partialRunStep);
                            }

                            serverSentEvent = runStep;
                            break;
                        case "thread.message.created":
                        case "thread.message.in_progress":
                        case "thread.message.delta":
                        case "thread.message.completed":
                        case "thread.message.incomplete":
                            var partialMessage = sseResponse.Deserialize<MessageResponse>(ssEvent, client);

                            if (message == null ||
                                message.Id != partialMessage.Id)
                            {
                                message = partialMessage;
                            }
                            else
                            {
                                message.AppendFrom(partialMessage);
                            }

                            serverSentEvent = message;
                            break;
                        case "error":
                            serverSentEvent = sseResponse.Deserialize<Error>(ssEvent, client);
                            break;
                        default:
                            // if not properly handled raise it up to caller to deal with it themselves.
                            serverSentEvent = ssEvent;
                            break;
                    }
                }
                catch (Exception e)
                {
                    @event = "error";
                    serverSentEvent = new Error(e);
                }
                finally
                {
                    await streamEventHandler.Invoke(@event, serverSentEvent).ConfigureAwait(false);
                }
            }, cancellationToken);

            if (run == null) { return null; }
            run = await run.WaitForStatusChangeAsync(timeout: -1, cancellationToken: cancellationToken).ConfigureAwait(false);
            run.SetResponseData(response.Headers, client);
            return run;
        }
    }
}
