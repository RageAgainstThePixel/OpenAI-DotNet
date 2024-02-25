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

        /// <summary>
        /// Creates a completion for the chat message.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<ChatResponse> GetCompletionAsync(ChatRequest chatRequest, CancellationToken cancellationToken = default)
        {
            using var jsonContent = JsonSerializer.Serialize(chatRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var response = await client.Client.PostAsync(GetUrl("/completions"), jsonContent, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(EnableDebug, jsonContent, null, cancellationToken).ConfigureAwait(false);
            return response.Deserialize<ChatResponse>(responseAsString, client);
        }

        /// <summary>
        /// Created a completion for the chat message and stream the results to the <paramref name="resultHandler"/> as they come in.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="resultHandler">An action to be called as each new result arrives.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<ChatResponse> StreamCompletionAsync(ChatRequest chatRequest, Action<ChatResponse> resultHandler, CancellationToken cancellationToken = default)
        {
            chatRequest.Stream = true;
            using var jsonContent = JsonSerializer.Serialize(chatRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var request = new HttpRequestMessage(HttpMethod.Post, GetUrl("/completions"));
            request.Content = jsonContent;
            using var response = await client.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(false, jsonContent, null, cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            ChatResponse chatResponse = null;
            using var responseStream = EnableDebug ? new MemoryStream() : null;

            if (responseStream != null)
            {
                await responseStream.WriteAsync("["u8.ToArray(), cancellationToken);
            }

            while (await reader.ReadLineAsync().ConfigureAwait(false) is { } streamData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!streamData.TryGetEventStreamData(out var eventData))
                {
                    // if response stream is not null, remove last comma
                    responseStream?.SetLength(responseStream.Length - 1);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(eventData)) { continue; }

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
                    chatResponse.CopyFrom(partialResponse);
                }

                resultHandler?.Invoke(partialResponse);
            }

            if (responseStream != null)
            {
                await responseStream.WriteAsync("]"u8.ToArray(), cancellationToken);
            }

            await response.CheckResponseAsync(EnableDebug, jsonContent, responseStream, cancellationToken).ConfigureAwait(false);

            if (chatResponse == null) { return null; }

            chatResponse.SetResponseData(response.Headers, client);
            resultHandler?.Invoke(chatResponse);
            return chatResponse;
        }

        /// <summary>
        /// Created a completion for the chat message and stream the results as they come in.<br/>
        /// If you are not using C# 8 supporting IAsyncEnumerable{T} or if you are using the .NET Framework,
        /// you may need to use <see cref="StreamCompletionAsync(ChatRequest, Action{ChatResponse}, CancellationToken)"/> instead.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async IAsyncEnumerable<ChatResponse> StreamCompletionEnumerableAsync(ChatRequest chatRequest, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            chatRequest.Stream = true;
            using var jsonContent = JsonSerializer.Serialize(chatRequest, OpenAIClient.JsonSerializationOptions).ToJsonStringContent();
            using var request = new HttpRequestMessage(HttpMethod.Post, GetUrl("/completions"));
            request.Content = jsonContent;
            using var response = await client.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(false, jsonContent, null, cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            ChatResponse chatResponse = null;
            using var responseStream = EnableDebug ? new MemoryStream() : null;

            if (responseStream != null)
            {
                await responseStream.WriteAsync("["u8.ToArray(), cancellationToken);
            }

            while (await reader.ReadLineAsync() is { } streamData)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!streamData.TryGetEventStreamData(out var eventData))
                {
                    // if response stream is not null, remove last comma
                    responseStream?.SetLength(responseStream.Length - 1);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(eventData)) { continue; }

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
                    chatResponse.CopyFrom(partialResponse);
                }

                yield return partialResponse;
            }

            if (responseStream != null)
            {
                await responseStream.WriteAsync("]"u8.ToArray(), cancellationToken);
            }

            await response.CheckResponseAsync(EnableDebug, jsonContent, responseStream, cancellationToken).ConfigureAwait(false);

            if (chatResponse == null) { yield break; }

            chatResponse.SetResponseData(response.Headers, client);
            yield return chatResponse;
        }
    }
}
