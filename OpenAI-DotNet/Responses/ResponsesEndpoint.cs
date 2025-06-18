// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="streamEventHandler">Optional, <see cref="Func{IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Response"/>.</returns>
        public async Task<Response> CreateModelResponseAsync(CreateResponseRequest request, Func<IServerSentEvent, Task> streamEventHandler = null, CancellationToken cancellationToken = default)
            => await CreateModelResponseAsync(request, streamEventHandler == null ? null : (_, e) => streamEventHandler(e), cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Creates a model response.
        /// Provide text or image inputs to generate text or JSON outputs.
        /// Have the model call your own custom code or use built-in tools like web search or file search to use your own data as input for the model's response.
        /// </summary>
        /// <param name="request"><see cref="CreateResponseRequest"/>.</param>
        /// <param name="streamEventHandler">Optional, <see cref="Func{String, IServerSentEvent, Task}"/> stream callback handler.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Response"/>.</returns>
        public async Task<Response> CreateModelResponseAsync(CreateResponseRequest request, Func<string, IServerSentEvent, Task> streamEventHandler, CancellationToken cancellationToken = default)
        {
            var endpoint = GetUrl();
            request.Stream = streamEventHandler != null;
            using var payload = JsonSerializer.Serialize(request, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();

            if (request.Stream)
            {
                return await StreamResponseAsync(endpoint, payload, streamEventHandler, cancellationToken).ConfigureAwait(false);
            }

            using var response = await HttpClient.PostAsync(endpoint, payload, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<Response>(EnableDebug, payload, client, cancellationToken).ConfigureAwait(false);
        }

        private async Task<Response> StreamResponseAsync(string endpoint, StringContent payload, Func<string, IServerSentEvent, Task> streamEventHandler, CancellationToken cancellationToken)
        {
            Response response = null;

            // ReSharper disable AccessToModifiedClosure
            using var streamResponse = await this.StreamEventsAsync(endpoint, payload, async (sseResponse, ssEvent) =>
            {
                IServerSentEvent serverSentEvent = null;
                var @event = ssEvent.Value.GetValue<string>();
                Console.WriteLine($"\"{@event}\": {ssEvent.ToJsonString()}");
                var @object = ssEvent.Data ?? ssEvent.Value;
                var text = @object["text"]?.GetValue<string>();
                var delta = @object["delta"]?.GetValue<string>();
                var itemId = @object["item_id"]?.GetValue<string>();
                var outputIndex = @object["output_index"]?.GetValue<int>();
                var contentIndex = @object["content_index"]?.GetValue<int>();

                // ReSharper disable once AccessToModifiedClosure
                try
                {
                    switch (@event)
                    {
                        case "response.created":
                        case "response.in_progress":
                        case "response.completed":
                        case "response.failed":
                        case "response.incomplete":
                            var partialResponse = sseResponse.Deserialize<Response>(@object["response"], client);

                            if (response == null)
                            {
                                response = partialResponse;
                            }
                            else
                            {
                                if (response.Id == partialResponse.Id)
                                {
                                    response = partialResponse;
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Response ID mismatch! Expected: {response.Id}, got: {partialResponse.Id}");
                                }
                            }

                            serverSentEvent = response;
                            break;
                        case "response.code_interpreter_call.code.delta":
                        case "response.code_interpreter_call.code.done":
                        case "response.code_interpreter_call.completed":
                        case "response.code_interpreter_call.in_progress":
                        case "response.code_interpreter_call.interpreting":
                            break;
                        case "response.content_part.added":
                        case "response.content_part.done":
                            var part = sseResponse.Deserialize<IResponseContent>(@object["part"], client);
                            var messageItem = (Message)response!.Output[outputIndex!.Value];

                            if (messageItem.Id != itemId)
                            {
                                throw new InvalidOperationException($"MessageItem ID mismatch! Expected: {messageItem.Id}, got: {itemId}");
                            }

                            messageItem.AddContentItem(part, contentIndex!.Value);

                            if (@event == "response.content_part.done")
                            {
                                serverSentEvent = part;
                            }
                            break;
                        case "response.file_search_call.completed":
                        case "response.file_search_call.in_progress":
                        case "response.file_search_call.searching":
                            break;
                        case "response.output_item.added":
                        case "response.output_item.done":
                            var item = sseResponse.Deserialize<IResponseItem>(@object["item"], client);
                            response!.InsertOutputItem(item, outputIndex!.Value);

                            if (@event == "response.output_item.done")
                            {
                                serverSentEvent = item;
                            }
                            break;
                        case "response.function_call_arguments.delta":
                        case "response.function_call_arguments.done":
                            var functionToolCall = (FunctionToolCall)response!.Output[outputIndex!.Value];

                            if (functionToolCall.Id != itemId)
                            {
                                throw new InvalidOperationException($"MessageItem ID mismatch! Expected: {functionToolCall.Id}, got: {itemId}");
                            }

                            if (!string.IsNullOrWhiteSpace(delta))
                            {
                                functionToolCall.Delta = delta;
                            }

                            break;
                        case "response.audio.delta":
                        case "response.audio.done":
                        case "response.audio.transcript.delta":
                        case "response.audio.transcript.done":
                        case "response.output_text.annotation.added":
                        case "response.output_text.delta":
                        case "response.output_text.done":
                        case "response.refusal.delta":
                        case "response.refusal.done":
                            messageItem = (Message)response!.Output[outputIndex!.Value];

                            if (messageItem.Id != itemId)
                            {
                                throw new InvalidOperationException($"MessageItem ID mismatch! Expected: {messageItem.Id}, got: {itemId}");
                            }

                            var contentItem = messageItem.Content[contentIndex!.Value];

                            switch (contentItem)
                            {
                                case AudioContent audioContent:
                                    AudioContent partialContent;
                                    switch (@event)
                                    {
                                        case "response.audio.delta":
                                            partialContent = new AudioContent(audioContent.Type, base64Data: delta);
                                            audioContent.AppendFrom(partialContent);
                                            serverSentEvent = partialContent;
                                            break;
                                        case "response.audio.transcript.delta":
                                            partialContent = new AudioContent(audioContent.Type, transcript: delta);
                                            audioContent.AppendFrom(partialContent);
                                            serverSentEvent = partialContent;
                                            break;
                                        case "response.audio.done":
                                        case "response.audio.transcript.done":
                                            serverSentEvent = audioContent;
                                            break;
                                        default:
                                            throw new InvalidOperationException($"Unexpected event type: {@event} for AudioContent.");
                                    }
                                    break;
                                case TextContent textContent:

                                    if (!string.IsNullOrWhiteSpace(text))
                                    {
                                        textContent.Text = text;
                                    }

                                    textContent.Delta = !string.IsNullOrWhiteSpace(delta) ? delta : null;

                                    var annotationIndex = @object["annotation_index"]?.GetValue<int>();

                                    if (annotationIndex.HasValue)
                                    {
                                        var annotation = sseResponse.Deserialize<Annotation>(@object["annotation"], client);
                                        textContent.InsertAnnotation(annotation, annotationIndex.Value);
                                    }

                                    serverSentEvent = textContent;
                                    break;
                                case RefusalContent refusalContent:
                                    var refusal = @object["refusal"]?.GetValue<string>();

                                    if (!string.IsNullOrWhiteSpace(refusal))
                                    {
                                        refusalContent.Refusal = refusal;
                                    }

                                    if (!string.IsNullOrWhiteSpace(delta))
                                    {
                                        refusalContent.Delta = delta;
                                    }

                                    serverSentEvent = refusalContent;
                                    break;
                            }
                            break;
                        case "response.reasoning_summary_part.added":
                        case "response.reasoning_summary_part.done":
                            var summaryIndex = @object["summary_index"]!.GetValue<int>();
                            var reasoningItem = (ReasoningItem)response!.Output[outputIndex!.Value];
                            var summaryItem = sseResponse.Deserialize<ReasoningSummary>(@object["part"], client);
                            reasoningItem.InsertSummary(summaryItem, summaryIndex);

                            if (@event == "response.reasoning_summary_part.done")
                            {
                                serverSentEvent = summaryItem;
                            }

                            break;
                        case "response.reasoning_summary_text.delta":
                        case "response.reasoning_summary_text.done":
                            summaryIndex = @object["summary_index"]!.GetValue<int>();
                            reasoningItem = (ReasoningItem)response!.Output[outputIndex!.Value];
                            summaryItem = reasoningItem.Summary[summaryIndex];

                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                summaryItem.Text = text;
                            }

                            summaryItem.Delta = !string.IsNullOrWhiteSpace(delta) ? delta : null;
                            serverSentEvent = summaryItem;
                            break;
                        case "response.web_search_call.completed":
                        case "response.web_search_call.in_progress":
                        case "response.web_search_call.searching":
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
                    serverSentEvent ??= ssEvent;
                    await streamEventHandler.Invoke(@event, serverSentEvent).ConfigureAwait(false);
                }
            }, cancellationToken).ConfigureAwait(false);
            // ReSharper restore AccessToModifiedClosure

            if (response == null) { return null; }
            response = await response.WaitForStatusChangeAsync(timeout: -1, cancellationToken: cancellationToken).ConfigureAwait(false);
            response.SetResponseData(streamResponse.Headers, client);
            return response;
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
            if (string.IsNullOrWhiteSpace(responseId))
            {
                throw new ArgumentException("Response ID cannot be null or empty.", nameof(responseId));
            }

            var queryParameters = new Dictionary<string, string>();

            if (include is { Length: > 0 })
            {
                queryParameters.Add("include", string.Join(",", include));
            }

            using var response = await HttpClient.GetAsync(GetUrl($"/{responseId}", queryParameters), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<Response>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a model response with the given ID.
        /// </summary>
        /// <param name="responseId">The ID of the response to delete.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns>True if the response was deleted, false otherwise.</returns>
        public async Task<bool> DeleteModelResponseAsync(string responseId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(responseId))
            {
                throw new ArgumentException("Response ID cannot be null or empty.", nameof(responseId));
            }
            using var response = await HttpClient.DeleteAsync(GetUrl($"/{responseId}"), cancellationToken).ConfigureAwait(false);
            var result = await response.DeserializeAsync<DeletedResponse>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
            return result?.Deleted ?? false;
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
            if (string.IsNullOrWhiteSpace(responseId))
            {
                throw new ArgumentException("Response ID cannot be null or empty.", nameof(responseId));
            }
            using var response = await HttpClient.PostAsync(GetUrl($"/{responseId}/cancel"), null!, cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<Response>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
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
            if (string.IsNullOrWhiteSpace(responseId))
            {
                throw new ArgumentException("Response ID cannot be null or empty.", nameof(responseId));
            }
            using var response = await HttpClient.GetAsync(GetUrl($"/{responseId}/input_items", query), cancellationToken).ConfigureAwait(false);
            return await response.DeserializeAsync<ListResponse<IResponseItem>>(EnableDebug, client, cancellationToken).ConfigureAwait(false);
        }
    }
}
