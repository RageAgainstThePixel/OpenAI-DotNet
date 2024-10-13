// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Realtime
{
    public sealed class RealtimeEndpoint : OpenAIBaseEndpoint
    {
        internal RealtimeEndpoint(OpenAIClient client) : base(client) { }

        protected override string Root => "realtime";

        public async Task<RealtimeSession> CreateSessionAsync(RealtimeSessionOptions options, CancellationToken cancellationToken = default)
        {
            var model = options.Model;
            var queryParameters = new Dictionary<string, string>();

            if (client.OpenAIClientSettings.IsAzureOpenAI)
            {
                queryParameters["deployment"] = model;
            }
            else
            {
                queryParameters["model"] = model;
            }

            var session = new RealtimeSession(client.CreateWebSocket(GetUrl(queryParameters: queryParameters)));
            await session.ConnectAsync(cancellationToken);
            return session;
        }
    }

    public sealed class RealtimeSession : IDisposable
    {
        public event Action<IRealtimeEvent> OnEventReceived;

        private readonly WebSocket websocketClient;

        internal RealtimeSession(WebSocket wsClient)
        {
            websocketClient = wsClient;
            websocketClient.OnMessage += OnMessage;
        }

        private void OnMessage(DataFrame dataFrame)
        {
            if (dataFrame.Type == OpCode.Text)
            {
                var message = JsonSerializer.Deserialize<IRealtimeEvent>(dataFrame.Text);
                OnEventReceived?.Invoke(message);
            }
        }

        ~RealtimeSession() => Dispose(false);

        #region IDisposable

        private bool isDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                websocketClient.Dispose();
                isDisposed = true;
            }
        }

        #endregion IDisposable

        #region Session Properties

        public string Id { get; private set; }

        #endregion Session Properties

        #region Internal Websockets

        internal Task ConnectAsync(CancellationToken cancellationToken)
        {
            return websocketClient.ConnectAsync(cancellationToken);
        }

        #endregion Internal Websockets
    }

    public interface IRealtimeEvent
    {
        public string EventId { get; }
        public string Type { get; }
        public string ToJsonString();
    }

    public sealed class SessionResponse : IRealtimeEvent
    {
        [JsonInclude]
        [JsonPropertyName("event_id")]
        public string EventId { get; }

        [JsonInclude]
        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonInclude]
        [JsonPropertyName("session")]
        public RealtimeSessionOptions Session { get; }

        public string ToJsonString() => JsonSerializer.Serialize(this, OpenAIClient.JsonSerializationOptions);
    }
}
