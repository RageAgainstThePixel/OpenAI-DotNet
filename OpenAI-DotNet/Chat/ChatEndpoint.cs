using System;
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
            var result = await Api.Client.PostAsync($"{GetEndpoint()}/completions", payload, cancellationToken);
            var resultAsString = await result.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<ChatResponse>(resultAsString, Api.JsonSerializationOptions);
        }

        // TODO Streaming endpoints
    }
}
