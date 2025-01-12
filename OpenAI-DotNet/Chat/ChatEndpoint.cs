// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Chat
{
    /// <summary>
    /// Given a chat conversation, the model will return a chat completion response.<br/>
    /// <see href="https://platform.openai.com/docs/api-reference/chat"/>
    /// </summary>
    public sealed class ChatEndpoint : OpenAIBaseEndpoint
    {
        /// <inheritdoc />
        public ChatEndpoint(OpenAIClient client) : base(client) { }

        /// <inheritdoc />
        protected override string Root => "chat";

        protected override bool? IsAzureDeployment => true;

        /// <summary>
        /// Creates a completion for the chat message.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<ChatResponse> GetCompletionAsync(ChatRequest chatRequest, CancellationToken cancellationToken = default)
        {
            using var payload = JsonSerializer.Serialize(chatRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl("/completions"), payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, payload, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ChatResponse>(responseAsString, client);
        }

        /// <summary>
        /// Creates a completion for the chat message.
        /// </summary>
        /// <typeparam name="T"><see cref="JsonSchema"/> to use for structured outputs.</typeparam>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<(T, ChatResponse)> GetCompletionAsync<T>(ChatRequest chatRequest, CancellationToken cancellationToken = default)
        {
            chatRequest.ResponseFormatObject = new ResponseFormatObject(typeof(T));
            var response = await GetCompletionAsync(chatRequest, cancellationToken).ConfigureAwait(false);
            var output = JsonSerializer.Deserialize<T>(response.FirstChoice, OpenAIClient.JsonSerializationOptions);
            return (output, response);
        }

        /// <summary>
        /// Created a completion for the chat message and stream the results to the <paramref name="resultHandler"/> as they come in.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="resultHandler">A <see cref="Action{ChatResponse}"/> to be invoked as each new result arrives.</param>
        /// <param name="streamUsage">
        /// Optional, If set, an additional chunk will be streamed before the 'data: [DONE]' message.
        /// The 'usage' field on this chunk shows the token usage statistics for the entire request,
        /// and the 'choices' field will always be an empty array. All other chunks will also include a 'usage' field,
        /// but with a null value.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<ChatResponse> StreamCompletionAsync(ChatRequest chatRequest, Action<ChatResponse> resultHandler, bool streamUsage = false, CancellationToken cancellationToken = default)
            => await StreamCompletionAsync(chatRequest, async response =>
            {
                resultHandler.Invoke(response);
                await Task.CompletedTask;
            }, streamUsage, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Created a completion for the chat message and stream the results to the <paramref name="resultHandler"/> as they come in.
        /// </summary>
        /// <typeparam name="T"><see cref="JsonSchema"/> to use for structured outputs.</typeparam>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="resultHandler">A <see cref="Action{ChatResponse}"/> to be invoked as each new result arrives.</param>
        /// <param name="streamUsage">
        /// Optional, If set, an additional chunk will be streamed before the 'data: [DONE]' message.
        /// The 'usage' field on this chunk shows the token usage statistics for the entire request,
        /// and the 'choices' field will always be an empty array. All other chunks will also include a 'usage' field,
        /// but with a null value.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<(T, ChatResponse)> StreamCompletionAsync<T>(ChatRequest chatRequest, Action<ChatResponse> resultHandler, bool streamUsage = false, CancellationToken cancellationToken = default)
            => await StreamCompletionAsync<T>(chatRequest, async response =>
            {
                resultHandler.Invoke(response);
                await Task.CompletedTask;
            }, streamUsage, cancellationToken);

        /// <summary>
        /// Created a completion for the chat message and stream the results to the <paramref name="resultHandler"/> as they come in.
        /// </summary>
        /// <typeparam name="T"><see cref="JsonSchema"/> to use for structured outputs.</typeparam>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="resultHandler">A <see cref="Func{ChatResponse, Task}"/> to to be invoked as each new result arrives.</param>
        /// <param name="streamUsage">
        /// Optional, If set, an additional chunk will be streamed before the 'data: [DONE]' message.
        /// The 'usage' field on this chunk shows the token usage statistics for the entire request,
        /// and the 'choices' field will always be an empty array. All other chunks will also include a 'usage' field,
        /// but with a null value.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<(T, ChatResponse)> StreamCompletionAsync<T>(ChatRequest chatRequest, Func<ChatResponse, Task> resultHandler, bool streamUsage = false, CancellationToken cancellationToken = default)
        {
            chatRequest.ResponseFormatObject = new ResponseFormatObject(typeof(T));
            var response = await StreamCompletionAsync(chatRequest, resultHandler, streamUsage, cancellationToken).ConfigureAwait(false);
            var output = JsonSerializer.Deserialize<T>(response.FirstChoice, OpenAIClient.JsonSerializationOptions);
            return (output, response);
        }

        /// <summary>
        /// Created a completion for the chat message and stream the results to the <paramref name="resultHandler"/> as they come in.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="resultHandler">A <see cref="Func{ChatResponse, Task}"/> to to be invoked as each new result arrives.</param>
        /// <param name="streamUsage">
        /// Optional, If set, an additional chunk will be streamed before the 'data: [DONE]' message.
        /// The 'usage' field on this chunk shows the token usage statistics for the entire request,
        /// and the 'choices' field will always be an empty array. All other chunks will also include a 'usage' field,
        /// but with a null value.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<ChatResponse> StreamCompletionAsync(ChatRequest chatRequest, Func<ChatResponse, Task> resultHandler, bool streamUsage = false, CancellationToken cancellationToken = default)
        {
            if (chatRequest == null) { throw new ArgumentNullException(nameof(chatRequest)); }
            if (resultHandler == null) { throw new ArgumentNullException(nameof(resultHandler)); }
            chatRequest.Stream = true;
            chatRequest.StreamOptions = streamUsage ? new StreamOptions() : null;
            ChatResponse chatResponse = null;
            using var payload = JsonSerializer.Serialize(chatRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await this.StreamEventsAsync(GetUrl("/completions"), payload, async (sseResponse, ssEvent) =>
            {
                var partialResponse = sseResponse.Deserialize<ChatResponse>(ssEvent, client);

                if (chatResponse == null)
                {
                    chatResponse = new ChatResponse(partialResponse);
                }
                else
                {
                    chatResponse.AppendFrom(partialResponse);
                }

                await resultHandler.Invoke(partialResponse).ConfigureAwait(false);
            }, cancellationToken);

            if (chatResponse == null) { return null; }
            chatResponse.SetResponseData(response.Headers, client);
            await resultHandler.Invoke(chatResponse).ConfigureAwait(false);
            return chatResponse;
        }

        /// <summary>
        /// Created a completion for the chat message and stream the results as they come in.<br/>
        /// If you are not using C# 8 supporting IAsyncEnumerable{T} or if you are using the .NET Framework,
        /// you may need to use <see cref="StreamCompletionAsync(ChatRequest, Action{ChatResponse}, bool, CancellationToken)"/> instead.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="streamUsage">
        /// Optional, If set, an additional chunk will be streamed before the 'data: [DONE]' message.
        /// The 'usage' field on this chunk shows the token usage statistics for the entire request,
        /// and the 'choices' field will always be an empty array. All other chunks will also include a 'usage' field,
        /// but with a null value.
        /// </param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async IAsyncEnumerable<ChatResponse> StreamCompletionEnumerableAsync(ChatRequest chatRequest, bool streamUsage = false, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            chatRequest.Stream = true;
            chatRequest.StreamOptions = streamUsage ? new StreamOptions() : null;
            using var payload = JsonSerializer.Serialize(chatRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var request = new HttpRequestMessage(HttpMethod.Post, GetUrl("/completions"));
            request.Content = payload;
            using var response = await client.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(false, payload, cancellationToken: cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            ChatResponse chatResponse = null;
            using var responseStream = EnableDebug ? new MemoryStream() : null;

            if (responseStream != null)
            {
                await responseStream.WriteAsync("["u8.ToArray(), cancellationToken);
            }

            while (await reader.ReadLineAsync(cancellationToken) is { } streamData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!streamData.TryGetEventStreamData(out var eventData))
                {
                    // if response stream is not null, remove last comma
                    responseStream?.SetLength(responseStream.Length - 1);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(eventData))
                {
                    continue;
                }

                if (responseStream != null)
                {
                    string data;

                    try
                    {
                        data = JsonNode.Parse(eventData)?.ToJsonString(OpenAIClient.JsonSerializationOptions);
                    }
                    catch
                    {
                        data = $"{{{eventData}}}";
                    }

                    await responseStream.WriteAsync(Encoding.UTF8.GetBytes($"{data},"), cancellationToken);
                }

                var partialResponse = response.Deserialize<ChatResponse>(eventData, client);

                if (chatResponse == null)
                {
                    chatResponse = new ChatResponse(partialResponse);
                }
                else
                {
                    chatResponse.AppendFrom(partialResponse);
                }

                yield return partialResponse;
            }

            if (responseStream != null)
            {
                await responseStream.WriteAsync("]"u8.ToArray(), cancellationToken);
            }

            await response.CheckResponseAsync(EnableDebug, payload, responseStream, null, cancellationToken).ConfigureAwait(false);

            if (chatResponse == null) { yield break; }

            chatResponse.SetResponseData(response.Headers, client);
            yield return chatResponse;
        }
    }
}
