using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Chat
{
    public sealed class ChatEndpoint : BaseEndPoint
    {
        public ChatEndpoint(OpenAIClient api) : base(api) { }

        protected override string GetEndpoint()
            => $"{Api.BaseUrl}chat";

        /// <summary>
        /// Creates a completion for the chat message
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        public async Task<ChatResponse> GetCompletionAsync(ChatRequest chatRequest, CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(chatRequest, Api.JsonSerializationOptions);
            var payload = json.ToJsonStringContent();
            var response = await Api.Client.PostAsync($"{GetEndpoint()}/completions", payload, cancellationToken).ConfigureAwait(false);
            var responseAsString = await response.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return response.DeserializeResponse<ChatResponse>(responseAsString, Api.JsonSerializationOptions);
        }


        /// <summary>
        /// Created a completion for the chat message and stream the results to the <paramref name="resultHandler"/> as they come in.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="resultHandler">An action to be called as each new result arrives.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task StreamCompletionAsync(ChatRequest chatRequest, Action<ChatResponse> resultHandler, CancellationToken cancellationToken = default)
        {
            chatRequest.Stream = true;
            var jsonContent = JsonSerializer.Serialize(chatRequest, Api.JsonSerializationOptions);
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{GetEndpoint()}/completions")
            {
                Content = jsonContent.ToJsonStringContent()
            };
            var response = await Api.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await response.CheckResponseAsync(cancellationToken);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);

            while (await reader.ReadLineAsync() is { } line)
            {
                if (line.StartsWith("data: "))
                {
                    line = line["data: ".Length..];
                }

                if (line == "[DONE]")
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(line))
                {
                    resultHandler(response.DeserializeResponse<ChatResponse>(line.Trim(), Api.JsonSerializationOptions));
                }
            }
        }

        /// <summary>
        /// Created a completion for the chat message and stream the results as they come in.<br/>
        /// If you are not using C# 8 supporting IAsyncEnumerable{T} or if you are using the .NET Framework,
        /// you may need to use <see cref="StreamCompletionAsync(ChatRequest, Action{ChatResponse}, CancellationToken)"/> instead.
        /// </summary>
        /// <param name="chatRequest">The chat request which contains the message content.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="ChatResponse"/>.</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async IAsyncEnumerable<ChatResponse> StreamCompletionEnumerableAsync(ChatRequest chatRequest, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            chatRequest.Stream = true;
            var jsonContent = JsonSerializer.Serialize(chatRequest, Api.JsonSerializationOptions);
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{GetEndpoint()}/completions")
            {
                Content = jsonContent.ToJsonStringContent()
            };
            var response = await Api.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            await response.CheckResponseAsync(cancellationToken);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new StreamReader(stream);

            while (await reader.ReadLineAsync() is { } line &&
                   !cancellationToken.IsCancellationRequested)
            {
                if (line.StartsWith("data: "))
                {
                    line = line["data: ".Length..];
                }

                if (line == "[DONE]")
                {
                    yield break;
                }

                if (!string.IsNullOrWhiteSpace(line))
                {
                    yield return response.DeserializeResponse<ChatResponse>(line.Trim(), Api.JsonSerializationOptions);
                }
            }
        }
    }
}
