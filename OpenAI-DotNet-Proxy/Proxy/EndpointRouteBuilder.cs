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
using System.Threading.Tasks;

namespace OpenAI.Proxy
{
    public static class EndpointRouteBuilder
    {
        // Copied from https://github.com/microsoft/reverse-proxy/blob/51d797986b1fea03500a1ad173d13a1176fb5552/src/ReverseProxy/Forwarder/RequestUtilities.cs#L61-L83
        private static readonly HashSet<string> excludedHeaders = new()
        {
            HeaderNames.Authorization,
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
        /// <param name="client"><see cref="OpenAIClient"/>.</param>
        /// <param name="authenticationFilter"><see cref="IAuthenticationFilter"/>.</param>
        /// <param name="routePrefix">Optional, custom route prefix. i.e. '/openai'.</param>
        public static void MapOpenAIEndpoints(this IEndpointRouteBuilder endpoints, OpenAIClient client, IAuthenticationFilter authenticationFilter, string routePrefix = "")
        {
            endpoints.Map($"{routePrefix}{client.Settings.BaseRequest}{{**endpoint}}", HandleRequest);
            return;

            async Task HandleRequest(HttpContext httpContext, string endpoint)
            {
                try
                {
                    if (httpContext.WebSockets.IsWebSocketRequest)
                    {
                        throw new InvalidOperationException("Websockets not supported");
                    }

                    await authenticationFilter.ValidateAuthenticationAsync(httpContext.Request.Headers).ConfigureAwait(false);
                    var method = new HttpMethod(httpContext.Request.Method);
                    var originalQuery = QueryHelpers.ParseQuery(httpContext.Request.QueryString.Value ?? "");
                    var modifiedQuery = new Dictionary<string, string>(originalQuery.Count);

                    foreach (var pair in originalQuery)
                    {
                        modifiedQuery[pair.Key] = pair.Value.FirstOrDefault();
                    }

                    if (client.Settings.IsAzureOpenAI)
                    {
                        modifiedQuery["api-version"] = client.Settings.ApiVersion;
                    }

                    var uri = new Uri(string.Format(
                        client.Settings.BaseRequestUrlFormat,
                        QueryHelpers.AddQueryString(endpoint, modifiedQuery)));
                    using var request = new HttpRequestMessage(method, uri);
                    request.Content = new StreamContent(httpContext.Request.Body);

                    foreach (var (key, value) in httpContext.Request.Headers)
                    {
                        if (excludedHeaders.Contains(key) ||
                            string.Equals(key, HeaderNames.ContentType, StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(key, HeaderNames.ContentLength, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        request.Headers.TryAddWithoutValidation(key, value.ToArray());
                    }

                    if (httpContext.Request.ContentType != null)
                    {
                        request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(httpContext.Request.ContentType);
                    }

                    var proxyResponse = await client.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, httpContext.RequestAborted).ConfigureAwait(false);
                    httpContext.Response.StatusCode = (int)proxyResponse.StatusCode;
                    httpContext.Response.ContentLength = proxyResponse.Content.Headers.ContentLength;
                    httpContext.Response.ContentType = proxyResponse.Content.Headers.ContentType?.ToString();

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

                    const string streamingContent = "text/event-stream";

                    if (httpContext.Response.ContentType != null &&
                        httpContext.Response.ContentType.Equals(streamingContent, StringComparison.OrdinalIgnoreCase))
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
                    Console.WriteLine($"{nameof(AuthenticationException)}: {authenticationException.Message}");
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync(authenticationException.Message).ConfigureAwait(false);
                }
                catch (WebSocketException webEx)
                {
                    Console.WriteLine($"{nameof(WebSocketException)} [{webEx.WebSocketErrorCode}] {webEx.Message}");
                    throw;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{nameof(Exception)}: {e.Message}");
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
        }
    }
}
