// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Extensions
{
    internal static class BaseEndpointExtensions
    {
        private const string ssePattern = @"(?:(?<type>[^:\n]*):)(?<value>[^\n]*)";

        private static Regex sseRegex = new(ssePattern);

        /// <summary>
        /// https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events
        /// </summary>
        public static async Task<HttpResponseMessage> StreamEventsAsync(this OpenAIBaseEndpoint baseEndpoint, string endpoint, StringContent payload, Func<HttpResponseMessage, ServerSentEvent, Task> eventCallback, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = payload;
            var response = await baseEndpoint.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(false, payload, cancellationToken: cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var events = new Stack<ServerSentEvent>();
            using var reader = new StreamReader(stream);
            var isEndOfStream = false;

            try
            {
                while (await reader.ReadLineAsync().ConfigureAwait(false) is { } streamData)
                {
                    if (isEndOfStream)
                    {
                        break;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    if (string.IsNullOrWhiteSpace(streamData))
                    {
                        continue;
                    }

                    var matches = sseRegex.Matches(streamData);

                    for (var i = 0; i < matches.Count; i++)
                    {
                        ServerSentEventKind type;
                        string value;
                        string data;

                        var match = matches[i];

                        // If the field type is not provided, treat it as a comment
                        type = ServerSentEvent.EventMap.GetValueOrDefault(match.Groups[nameof(type)].Value.Trim(), ServerSentEventKind.Comment);
                        // The UTF-8 decode algorithm strips one leading UTF-8 Byte Order Mark (BOM), if any.
                        value = match.Groups[nameof(value)].Value.TrimStart(' ');
                        data = match.Groups[nameof(data)].Value;

                        const string doneTag = "[DONE]";
                        const string doneEvent = "done";

                        // if either value or data equals doneTag then stop processing events.
                        if (value.Equals(doneTag) || data.Equals(doneTag) || value.Equals(doneEvent))
                        {
                            isEndOfStream = true;
                            break;
                        }

                        var @event = new ServerSentEvent(type);

                        try
                        {
                            @event.Value = JsonNode.Parse(value);
                        }
                        catch
                        {
                            @event.Value = value;
                        }

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            try
                            {
                                @event.Data = JsonNode.Parse(data);
                            }
                            catch
                            {
                                @event.Data = string.IsNullOrWhiteSpace(data) ? null : data;
                            }
                        }

                        if (type == ServerSentEventKind.Data &&
                            events.Count > 0 &&
                            events.Peek().Event == ServerSentEventKind.Event)
                        {
                            var previousEvent = events.Pop();
                            previousEvent.Data = @event.Value;
                            events.Push(previousEvent);
                            await eventCallback.Invoke(response, previousEvent).ConfigureAwait(false);
                        }
                        else
                        {
                            events.Push(@event);

                            if (type != ServerSentEventKind.Event)
                            {
                                await eventCallback.Invoke(response, @event).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
            finally
            {
                await response.CheckResponseAsync(baseEndpoint.EnableDebug, payload, null, events.Reverse().ToList(), cancellationToken).ConfigureAwait(false);
            }

            return response;
        }
    }
}
