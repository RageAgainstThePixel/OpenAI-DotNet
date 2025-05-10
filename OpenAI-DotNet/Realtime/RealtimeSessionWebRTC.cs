// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SIPSorcery.Net;
using System.Linq;
using System.Text;

namespace OpenAI.Realtime
{
    public sealed class RealtimeSessionWebRTC : IDisposable
    {
        /// <summary>
        /// Enable or disable logging.
        /// </summary>
        public bool EnableDebug { get; set; }

        /// <summary>
        /// The timeout in seconds to wait for a response from the server.
        /// </summary>
        public int EventTimeout { get; set; } = 30;

        /// <summary>
        /// The configuration options for the session.
        /// </summary>
        public SessionConfiguration Configuration { get; internal set; }

        #region Internal

        internal event Action<IServerEvent> OnEventReceived;

        internal event Action<Exception> OnError;

        private readonly RTCPeerConnection peerConnection;
        private readonly ConcurrentQueue<IRealtimeEvent> events = new();
        private readonly object eventLock = new();

        internal RealtimeSessionWebRTC(RTCPeerConnection pc, bool enableDebug)
        {
            peerConnection = pc;
            EnableDebug = enableDebug;

            SetPeerConnectionEventHandlers(pc);
        }

        private void SetPeerConnectionEventHandlers(RTCPeerConnection pc)
        {
            var dataChannel = pc.DataChannels.FirstOrDefault();

            if (dataChannel != null)
            {
                dataChannel.onmessage += OnMessage;
            }
        }

        private void OnMessage(RTCDataChannel dc, DataChannelPayloadProtocols protocol, byte[] data)
        {
            var rawMessage = Encoding.UTF8.GetString(data);

            if (EnableDebug)
            {
                Console.WriteLine(rawMessage);
            }

            try
            {
                var @event = JsonSerializer.Deserialize<IServerEvent>(rawMessage, OpenAIClient.JsonSerializationOptions);

                lock (eventLock)
                {
                    events.Enqueue(@event);
                }

                OnEventReceived?.Invoke(@event);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                OnError?.Invoke(e);
            }
        }

        ~RealtimeSessionWebRTC() => Dispose(false);

        #region IDisposable

        private bool isDisposed;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                var dataChannel = peerConnection?.DataChannels.FirstOrDefault();

                if (dataChannel != null)
                {
                    dataChannel.onmessage -= OnMessage;
                }

                peerConnection.Dispose();
                isDisposed = true;
            }
        }

        #endregion IDisposable

        #endregion

        /// <summary>
        /// Send a client event to the server.
        /// </summary>
        /// <typeparam name="T"><see cref="IClientEvent"/> to send to the server.</typeparam>
        /// <param name="event">The event to send.</param>
        public async void Send<T>(T @event) where T : IClientEvent
            => await SendAsync(@event).ConfigureAwait(false);

        /// <summary>
        /// Send a client event to the server.
        /// </summary>
        /// <typeparam name="T"><see cref="IClientEvent"/> to send to the server.</typeparam>
        /// <param name="event">The event to send.</param>
        /// <param name="@event">Optional, <see cref="Action{IServerEvent}"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task{IServerEvent}"/>.</returns>
        public async Task<IServerEvent> SendAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IClientEvent
            => await SendAsync(@event, null, cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// Send a client event to the server.
        /// </summary>
        /// <typeparam name="T"><see cref="IClientEvent"/> to send to the server.</typeparam>
        /// <param name="event">The event to send.</param>
        /// <param name="sessionEvents">Optional, <see cref="Action{IServerEvent}"/>.</param>
        /// <param name="cancellationToken">Optional, <see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task{IServerEvent}"/>.</returns>
        public async Task<IServerEvent> SendAsync<T>(T @event, Action<IServerEvent> sessionEvents, CancellationToken cancellationToken = default) where T : IClientEvent
        {
            if (peerConnection.connectionState != RTCPeerConnectionState.connected)
            {
                throw new Exception($"WebRTC connection is not open! {peerConnection.connectionState}");
            }

            IClientEvent clientEvent = @event;
            var payload = clientEvent.ToJsonString();

            if (EnableDebug)
            {
                if (@event is not InputAudioBufferAppendRequest)
                {
                    Console.WriteLine(payload);
                }
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(EventTimeout));
            using var eventCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
            var tcs = new TaskCompletionSource<IServerEvent>();
            eventCts.Token.Register(() => tcs.TrySetCanceled());
            OnEventReceived += EventCallback;

            lock (eventLock)
            {
                events.Enqueue(clientEvent);
            }

            var eventId = Guid.NewGuid().ToString("N");

            if (EnableDebug)
            {
                if (@event is not InputAudioBufferAppendRequest)
                {
                    Console.WriteLine($"[{eventId}] sending {clientEvent.Type}");
                }
            }

            peerConnection.DataChannels.First().send(payload);

            if (EnableDebug)
            {
                if (@event is not InputAudioBufferAppendRequest)
                {
                    Console.WriteLine($"[{eventId}] sent {clientEvent.Type}");
                }
            }

            if (@event is InputAudioBufferAppendRequest)
            {
                // no response for this client event
                return default;
            }

            var response = await tcs.Task.WithCancellation(eventCts.Token).ConfigureAwait(false);

            if (EnableDebug)
            {
                Console.WriteLine($"[{eventId}] received {response.Type}");
            }

            return response;

            void EventCallback(IServerEvent serverEvent)
            {
                sessionEvents?.Invoke(serverEvent);

                try
                {
                    if (serverEvent is RealtimeEventError serverError)
                    {
                        tcs.TrySetException(serverError);
                        OnEventReceived -= EventCallback;
                        return;
                    }

                    switch (clientEvent)
                    {
                        case UpdateSessionRequest when serverEvent is SessionResponse sessionResponse:
                            Configuration = sessionResponse.SessionConfiguration;
                            Complete();
                            return;
                        case InputAudioBufferCommitRequest when serverEvent is InputAudioBufferCommittedResponse:
                        case InputAudioBufferClearRequest when serverEvent is InputAudioBufferClearedResponse:
                        case ConversationItemCreateRequest when serverEvent is ConversationItemCreatedResponse:
                        case ConversationItemTruncateRequest when serverEvent is ConversationItemTruncatedResponse:
                        case ConversationItemDeleteRequest when serverEvent is ConversationItemDeletedResponse:
                            Complete();
                            return;
                        case CreateResponseRequest when serverEvent is RealtimeResponse serverResponse:
                            {
                                if (serverResponse.Response.Status == RealtimeResponseStatus.InProgress)
                                {
                                    return;
                                }

                                if (serverResponse.Response.Status != RealtimeResponseStatus.Completed)
                                {
                                    tcs.TrySetException(new Exception(serverResponse.Response.StatusDetails.Error?.ToString() ?? serverResponse.Response.StatusDetails.Reason));
                                }
                                else
                                {
                                    Complete();
                                }

                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return;

                void Complete()
                {
                    if (EnableDebug)
                    {
                        Console.WriteLine($"{clientEvent.Type} -> {serverEvent.Type}");
                    }

                    tcs.TrySetResult(serverEvent);
                    OnEventReceived -= EventCallback;
                }
            }
        }
    }
}
