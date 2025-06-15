// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Proxy
{
    public static class EndpointRouteBuilder
    {
        // Copied from https://github.com/microsoft/reverse-proxy/blob/51d797986b1fea03500a1ad173d13a1176fb5552/src/ReverseProxy/Forwarder/RequestUtilities.cs#L61-L83
        private static readonly HashSet<string> excludedHeaders = new()
        {
            HeaderNames.Connection,
            HeaderNames.TransferEncoding,
            HeaderNames.KeepAlive,
            HeaderNames.Upgrade,
            HeaderNames.Host,
            HeaderNames.SecWebSocketKey,
            HeaderNames.SecWebSocketVersion,
            "Proxy-Connection",
            "Proxy-Authenticate",
            "Proxy-Authentication-Info",
            "Proxy-Authorization",
            "Proxy-Features",
            "Proxy-Instruction",
            "Security-Scheme",
            "ALPN",
            "Close",
            "Set-Cookie",
            HeaderNames.TE,
#if NET
            HeaderNames.AltSvc,
#else
            "Alt-Svc",
#endif
        };

        /// <summary>
        /// Maps the <see cref="OpenAIClient"/> endpoints.
        /// </summary>
        /// <param name="endpoints"><see cref="IEndpointRouteBuilder"/>.</param>
        /// <param name="openAIClient"><see cref="OpenAIClient"/>.</param>
        /// <param name="authenticationFilter"><see cref="IAuthenticationFilter"/>.</param>
        /// <param name="routePrefix">Optional, custom route prefix. i.e. '/openai'.</param>
        public static void MapOpenAIEndpoints(this IEndpointRouteBuilder endpoints, OpenAIClient openAIClient, IAuthenticationFilter authenticationFilter, string routePrefix = "")
        {
            endpoints.Map($"{routePrefix}{openAIClient.Settings.BaseRequest}{{**endpoint}}", HandleRequest);
            return;

            async Task HandleRequest(HttpContext httpContext, string endpoint)
            {
                try
                {
                    if (httpContext.WebSockets.IsWebSocketRequest)
                    {
                        await ProcessWebSocketRequest(httpContext, endpoint).ConfigureAwait(false);
                        return;
                    }

                    await authenticationFilter.ValidateAuthenticationAsync(httpContext.Request.Headers).ConfigureAwait(false);
                    var method = new HttpMethod(httpContext.Request.Method);
                    var originalQuery = QueryHelpers.ParseQuery(httpContext.Request.QueryString.Value ?? "");
                    var modifiedQuery = new Dictionary<string, string>(originalQuery.Count);

                    foreach (var pair in originalQuery)
                    {
                        modifiedQuery[pair.Key] = pair.Value.FirstOrDefault();
                    }

                    if (openAIClient.Settings.IsAzureOpenAI)
                    {
                        modifiedQuery["api-version"] = openAIClient.Settings.ApiVersion;
                    }

                    var uri = new Uri(string.Format(
                        openAIClient.Settings.BaseRequestUrlFormat,
                        QueryHelpers.AddQueryString(endpoint, modifiedQuery)
                    ));

                    using var request = new HttpRequestMessage(method, uri);
                    request.Content = new StreamContent(httpContext.Request.Body);

                    if (httpContext.Request.ContentType != null)
                    {
                        request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(httpContext.Request.ContentType);
                    }

                    var proxyResponse = await openAIClient.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, httpContext.RequestAborted).ConfigureAwait(false);
                    httpContext.Response.StatusCode = (int)proxyResponse.StatusCode;

                    foreach (var (key, value) in proxyResponse.Headers)
                    {
                        if (excludedHeaders.Contains(key)) { continue; }
                        httpContext.Response.Headers[key] = value.ToArray();
                    }

                    foreach (var (key, value) in proxyResponse.Content.Headers)
                    {
                        if (excludedHeaders.Contains(key)) { continue; }
                        httpContext.Response.Headers[key] = value.ToArray();
                    }

                    httpContext.Response.ContentType = proxyResponse.Content.Headers.ContentType?.ToString() ?? string.Empty;
                    const string streamingContent = "text/event-stream";

                    if (httpContext.Response.ContentType.Equals(streamingContent))
                    {
                        var stream = await proxyResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        await WriteServerStreamEventsAsync(httpContext, stream).ConfigureAwait(false);
                    }
                    else
                    {
                        await proxyResponse.Content.CopyToAsync(httpContext.Response.Body, httpContext.RequestAborted).ConfigureAwait(false);
                    }
                }
                catch (AuthenticationException authenticationException)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync(authenticationException.Message).ConfigureAwait(false);
                }
                catch (WebSocketException)
                {
                    // ignore
                    throw;
                }
                catch (Exception e)
                {
                    if (httpContext.Response.HasStarted) { throw; }
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    var response = JsonSerializer.Serialize(new { error = new { e.Message, e.StackTrace } });
                    await httpContext.Response.WriteAsync(response).ConfigureAwait(false);
                }

                static async Task WriteServerStreamEventsAsync(HttpContext httpContext, Stream contentStream)
                {
                    var responseStream = httpContext.Response.Body;
                    await contentStream.CopyToAsync(responseStream, httpContext.RequestAborted).ConfigureAwait(false);
                    await responseStream.FlushAsync(httpContext.RequestAborted).ConfigureAwait(false);
                }
            }

            async Task ProcessWebSocketRequest(HttpContext httpContext, string endpoint)
            {
                using var clientWebsocket = await httpContext.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);

                try
                {
                    await authenticationFilter.ValidateAuthenticationAsync(httpContext.Request.Headers).ConfigureAwait(false);
                }
                catch (AuthenticationException authenticationException)
                {
                    var message = JsonSerializer.Serialize(new
                    {
                        type = "error",
                        error = new
                        {
                            type = "invalid_request_error",
                            code = "invalid_session_token",
                            message = authenticationException.Message
                        }
                    });
                    await clientWebsocket.SendAsync(System.Text.Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, httpContext.RequestAborted).ConfigureAwait(false);
                    await clientWebsocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, authenticationException.Message, httpContext.RequestAborted).ConfigureAwait(false);
                    return;
                }

                if (endpoint.EndsWith("echo"))
                {
                    await EchoAsync(clientWebsocket, httpContext.RequestAborted);
                    return;
                }

                using var hostWebsocket = new ClientWebSocket();

                foreach (var header in openAIClient.WebsocketHeaders)
                {
                    hostWebsocket.Options.SetRequestHeader(header.Key, header.Value);
                }

                var uri = new Uri(string.Format(
                    openAIClient.Settings.BaseWebSocketUrlFormat,
                    $"{endpoint}{httpContext.Request.QueryString}"
                ));
                await hostWebsocket.ConnectAsync(uri, httpContext.RequestAborted).ConfigureAwait(false);
                var receive = ProxyWebSocketMessages(clientWebsocket, hostWebsocket, httpContext.RequestAborted);
                var send = ProxyWebSocketMessages(hostWebsocket, clientWebsocket, httpContext.RequestAborted);
                await Task.WhenAll(receive, send).ConfigureAwait(false);
                return;

                async Task ProxyWebSocketMessages(WebSocket fromSocket, WebSocket toSocket, CancellationToken cancellationToken)
                {
                    var buffer = new byte[1024 * 4];
                    var memoryBuffer = buffer.AsMemory();

                    while (fromSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                    {
                        var result = await fromSocket.ReceiveAsync(memoryBuffer, cancellationToken).ConfigureAwait(false);

                        if (fromSocket.CloseStatus.HasValue || result.MessageType == WebSocketMessageType.Close)
                        {
                            await toSocket.CloseOutputAsync(fromSocket.CloseStatus ?? WebSocketCloseStatus.NormalClosure, fromSocket.CloseStatusDescription ?? "Closing", cancellationToken).ConfigureAwait(false);
                            break;
                        }

                        await toSocket.SendAsync(memoryBuffer[..result.Count], result.MessageType, result.EndOfMessage, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            static async Task EchoAsync(WebSocket webSocket, CancellationToken cancellationToken)
            {
                var buffer = new byte[1024 * 4];
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveResult.Count), receiveResult.MessageType, receiveResult.EndOfMessage, cancellationToken);
                    receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                }

                await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, cancellationToken);
            }
        }
    }
}
