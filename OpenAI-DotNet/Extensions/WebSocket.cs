// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Extensions
{
    internal class WebSocket : IDisposable
    {
        public WebSocket(string url, IReadOnlyDictionary<string, string> requestHeaders = null, IReadOnlyList<string> subProtocols = null)
            : this(new Uri(url), requestHeaders, subProtocols)
        {
        }

        public WebSocket(Uri uri, IReadOnlyDictionary<string, string> requestHeaders = null, IReadOnlyList<string> subProtocols = null)
        {
            var protocol = uri.Scheme;

            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
            {
                throw new ArgumentException($"Unsupported protocol: {protocol}");
            }

            Address = uri;
            RequestHeaders = requestHeaders ?? new Dictionary<string, string>();
            SubProtocols = subProtocols ?? new List<string>();
            CreateWebsocketAsync = (_, _) => Task.FromResult<System.Net.WebSockets.WebSocket>(new ClientWebSocket());
            RunMessageQueue();
        }

        private async void RunMessageQueue()
        {
            while (_semaphore != null)
            {
                while (_events.TryDequeue(out var action))
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        OnError?.Invoke(e);
                    }
                }

                await Task.Delay(16);
            }
        }

        ~WebSocket() => Dispose(false);

        #region IDisposable

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    if (State == State.Open)
                    {
                        CloseAsync().Wait();
                    }

                    _socket?.Dispose();
                    _socket = null;

                    _lifetimeCts?.Cancel();
                    _lifetimeCts?.Dispose();
                    _lifetimeCts = null;

                    _semaphore?.Dispose();
                    _semaphore = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        public event Action OnOpen;

        public event Action<DataFrame> OnMessage;

        public event Action<Exception> OnError;

        public event Action<CloseStatusCode, string> OnClose;

        public Uri Address { get; }

        public IReadOnlyDictionary<string, string> RequestHeaders { get; }

        public IReadOnlyList<string> SubProtocols { get; }

        public State State => _socket?.State switch
        {
            WebSocketState.Connecting => State.Connecting,
            WebSocketState.Open => State.Open,
            WebSocketState.CloseSent or WebSocketState.CloseReceived => State.Closing,
            _ => State.Closed
        };

        private readonly object _lock = new();
        private System.Net.WebSockets.WebSocket _socket;
        private SemaphoreSlim _semaphore = new(1, 1);
        private CancellationTokenSource _lifetimeCts;
        private readonly ConcurrentQueue<Action> _events = new();

        public async void Connect()
            => await ConnectAsync().ConfigureAwait(false);

        // used for unit testing websocket server
        internal Func<Uri, CancellationToken, Task<System.Net.WebSockets.WebSocket>> CreateWebsocketAsync;

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (State == State.Open)
                {
                    Console.WriteLine("Websocket is already open!");
                    return;
                }

                // ReSharper disable once MethodHasAsyncOverload
                _lifetimeCts?.Cancel();
                _lifetimeCts?.Dispose();
                _lifetimeCts = new CancellationTokenSource();
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token, cancellationToken);

                _socket = await CreateWebsocketAsync.Invoke(Address, cts.Token).ConfigureAwait(false);

                if (_socket is ClientWebSocket clientWebSocket)
                {
                    foreach (var requestHeader in RequestHeaders)
                    {
                        clientWebSocket.Options.SetRequestHeader(requestHeader.Key, requestHeader.Value);
                    }

                    foreach (var subProtocol in SubProtocols)
                    {
                        clientWebSocket.Options.AddSubProtocol(subProtocol);
                    }

                    await clientWebSocket.ConnectAsync(Address, cts.Token).ConfigureAwait(false);
                }

                _events.Enqueue(() => OnOpen?.Invoke());
                var buffer = new Memory<byte>(new byte[8192]);

                while (State == State.Open)
                {
                    ValueWebSocketReceiveResult result;
                    using var stream = new MemoryStream();

                    do
                    {
                        result = await _socket.ReceiveAsync(buffer, cts.Token).ConfigureAwait(false);
                        stream.Write(buffer.Span[..result.Count]);
                    } while (!result.EndOfMessage);

                    await stream.FlushAsync(cts.Token).ConfigureAwait(false);
                    var memory = new ReadOnlyMemory<byte>(stream.GetBuffer(), 0, (int)stream.Length);

                    if (result.MessageType != WebSocketMessageType.Close)
                    {
                        _events.Enqueue(() => OnMessage?.Invoke(new DataFrame((OpCode)(int)result.MessageType, memory)));
                    }
                    else
                    {
                        await CloseAsync(cancellationToken: CancellationToken.None).ConfigureAwait(false);
                        break;
                    }
                }

                try
                {
                    await _semaphore.WaitAsync(CancellationToken.None).ConfigureAwait(false);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case TaskCanceledException:
                    case OperationCanceledException:
                        break;
                    default:
                        Console.WriteLine(e);
                        _events.Enqueue(() => OnError?.Invoke(e));
                        _events.Enqueue(() => OnClose?.Invoke(CloseStatusCode.AbnormalClosure, e.Message));
                        break;
                }
            }
        }

        public async Task SendAsync(string text, CancellationToken cancellationToken = default)
            => await Internal_SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Text, cancellationToken).ConfigureAwait(false);

        public async Task SendAsync(ArraySegment<byte> data, CancellationToken cancellationToken = default)
            => await Internal_SendAsync(data, WebSocketMessageType.Binary, cancellationToken).ConfigureAwait(false);

        private async Task Internal_SendAsync(ArraySegment<byte> data, WebSocketMessageType opCode, CancellationToken cancellationToken)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(_lifetimeCts.Token, cancellationToken);
                await _semaphore.WaitAsync(cts.Token).ConfigureAwait(false);

                if (State != State.Open)
                {
                    throw new InvalidOperationException("WebSocket is not ready!");
                }

                await _socket.SendAsync(data, opCode, true, cts.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case TaskCanceledException:
                    case OperationCanceledException:
                        break;
                    default:
                        Console.WriteLine(e);
                        _events.Enqueue(() => OnError?.Invoke(e));
                        break;
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async void Close()
            => await CloseAsync();

        public async Task CloseAsync(CloseStatusCode code = CloseStatusCode.Normal, string reason = "", CancellationToken cancellationToken = default)
        {
            try
            {
                if (State == State.Open)
                {
                    await _socket.CloseAsync((WebSocketCloseStatus)(int)code, reason, cancellationToken).ConfigureAwait(false);
                    _events.Enqueue(() => OnClose?.Invoke(code, reason));
                }
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ObjectDisposedException:
                    case TaskCanceledException:
                    case OperationCanceledException:
                        _events.Enqueue(() => OnClose?.Invoke(code, reason));
                        break;
                    default:
                        Console.WriteLine(e);
                        _events.Enqueue(() => OnError?.Invoke(e));
                        break;
                }
            }
        }
    }

    internal class DataFrame
    {
        public OpCode Type { get; }

        public ReadOnlyMemory<byte> Data { get; }

        public string Text { get; }

        public DataFrame(OpCode type, ReadOnlyMemory<byte> data)
        {
            Type = type;
            Data = data;
            Text = type == OpCode.Text
                ? Encoding.UTF8.GetString(data.Span)
                : string.Empty;
        }
    }

    internal enum CloseStatusCode : ushort
    {
        /// <summary>
        /// Indicates a normal closure, meaning that the purpose for which the connection was established has been fulfilled.
        /// </summary>
        Normal = 1000,
        /// <summary>
        /// Indicates that an endpoint is "going away", such as a server going down or a browser having navigated away from a page.
        /// </summary>
        GoingAway = 1001,
        /// <summary>
        /// Indicates that an endpoint is terminating the connection due to a protocol error.
        /// </summary>
        ProtocolError = 1002,
        /// <summary>
        /// Indicates that an endpoint is terminating the connection because it has received a type of data it cannot accept
        /// (e.g., an endpoint that understands only text data MAY send this if it receives a binary message).
        /// </summary>
        UnsupportedData = 1003,
        /// <summary>
        /// Reserved and MUST NOT be set as a status code in a Close control frame by an endpoint.<para/>
        /// The specific meaning might be defined in the future.
        /// </summary>
        Reserved = 1004,
        /// <summary>
        /// Reserved and MUST NOT be set as a status code in a Close control frame by an endpoint.<para/>
        /// It is designated for use in applications expecting a status code to indicate that no status code was actually present.
        /// </summary>
        NoStatus = 1005,
        /// <summary>
        /// Reserved and MUST NOT be set as a status code in a Close control frame by an endpoint.<para/>
        /// It is designated for use in applications expecting a status code to indicate that the connection was closed abnormally,
        /// e.g., without sending or receiving a Close control frame.
        /// </summary>
        AbnormalClosure = 1006,
        /// <summary>
        /// Indicates that an endpoint is terminating the connection because it has received data within a message
        /// that was not consistent with the type of the message.
        /// </summary>
        InvalidPayloadData = 1007,
        /// <summary>
        /// Indicates that an endpoint is terminating the connection because it received a message that violates its policy.
        /// This is a generic status code that can be returned when there is no other more suitable status code (e.g., 1003 or 1009)
        /// or if there is a need to hide specific details about the policy.
        /// </summary>
        PolicyViolation = 1008,
        /// <summary>
        /// Indicates that an endpoint is terminating the connection because it has received a message that is too big for it to process.
        /// </summary>
        TooBigToProcess = 1009,
        /// <summary>
        /// Indicates that an endpoint (client) is terminating the connection because it has expected the server to negotiate
        /// one or more extension, but the server didn't return them in the response message of the WebSocket handshake.
        /// The list of extensions that are needed SHOULD appear in the /reason/ part of the Close frame. Note that this status code
        /// is not used by the server, because it can fail the WebSocket handshake instead.
        /// </summary>
        MandatoryExtension = 1010,
        /// <summary>
        /// Indicates that a server is terminating the connection because it encountered an unexpected condition that prevented it from fulfilling the request.
        /// </summary>
        ServerError = 1011,
        /// <summary>
        /// Reserved and MUST NOT be set as a status code in a Close control frame by an endpoint.<para/>
        /// It is designated for use in applications expecting a status code to indicate that the connection was closed due to a failure to perform a TLS handshake
        /// (e.g., the server certificate can't be verified).
        /// </summary>
        TlsHandshakeFailure = 1015
    }

    internal enum OpCode
    {
        Text,
        Binary
    }

    internal enum State : ushort
    {
        /// <summary>
        /// The connection has not yet been established.
        /// </summary>
        Connecting = 0,
        /// <summary>
        /// The connection has been established and communication is possible.
        /// </summary>
        Open = 1,
        /// <summary>
        /// The connection is going through the closing handshake or close has been requested.
        /// </summary>
        Closing = 2,
        /// <summary>
        /// The connection has been closed or could not be opened.
        /// </summary>
        Closed = 3
    }
}
