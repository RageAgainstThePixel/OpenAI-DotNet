// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
            HeaderNames.Connection,
            HeaderNames.TransferEncoding,
            HeaderNames.KeepAlive,
            HeaderNames.Upgrade,
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
            endpoints.Map($"{routePrefix}{openAIClient.OpenAIClientSettings.BaseRequest}{{**endpoint}}", HandleRequest);

            async Task HandleRequest(HttpContext httpContext, string endpoint)
            {
                try
                {
                    // ReSharper disable once MethodHasAsyncOverload
                    // just in case either method is implemented we call it twice.
                    authenticationFilter.ValidateAuthentication(httpContext.Request.Headers);
                    await authenticationFilter.ValidateAuthenticationAsync(httpContext.Request.Headers);

                    var method = new HttpMethod(httpContext.Request.Method);
                    var uri = new Uri(string.Format(
                            openAIClient.OpenAIClientSettings.BaseRequestUrlFormat,
                            $"{endpoint}{httpContext.Request.QueryString}"
                        ));
                    using var request = new HttpRequestMessage(method, uri);
                    request.Content = new StreamContent(httpContext.Request.Body);

                    if (httpContext.Request.ContentType != null)
                    {
                        request.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(httpContext.Request.ContentType);
                    }

                    var proxyResponse = await openAIClient.Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    httpContext.Response.StatusCode = (int)proxyResponse.StatusCode;

                    foreach (var (key, value) in proxyResponse.Headers)
                    {
                        if (excludedHeaders.Contains(key))
                        {
                            continue;
                        }

                        httpContext.Response.Headers[key] = value.ToArray();
                    }

                    foreach (var (key, value) in proxyResponse.Content.Headers)
                    {
                        if (excludedHeaders.Contains(key))
                        {
                            continue;
                        }

                        httpContext.Response.Headers[key] = value.ToArray();
                    }

                    httpContext.Response.ContentType = proxyResponse.Content.Headers.ContentType?.ToString() ?? string.Empty;
                    const string streamingContent = "text/event-stream";

                    if (httpContext.Response.ContentType.Equals(streamingContent))
                    {
                        var stream = await proxyResponse.Content.ReadAsStreamAsync();
                        await WriteServerStreamEventsAsync(httpContext, stream);
                    }
                    else
                    {
                        await proxyResponse.Content.CopyToAsync(httpContext.Response.Body);
                    }
                }
                catch (AuthenticationException authenticationException)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync(authenticationException.Message);
                }
                catch (Exception e)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    var response = JsonSerializer.Serialize(new { error = new { e.Message, e.StackTrace } });
                    await httpContext.Response.WriteAsync(response);
                }

                static async Task WriteServerStreamEventsAsync(HttpContext httpContext, Stream contentStream)
                {
                    var responseStream = httpContext.Response.Body;
                    await contentStream.CopyToAsync(responseStream, httpContext.RequestAborted);
                    await responseStream.FlushAsync(httpContext.RequestAborted);
                }
            }
        }
    }
}
