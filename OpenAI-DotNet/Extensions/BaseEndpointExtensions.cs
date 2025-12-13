// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Extensions
{
    internal static class BaseEndpointExtensions
    {
        private const char Space = ' ';
        private const char Bom = '\uFEFF';
        private const char NewLine = '\n';
        private const char Return = '\r';
        private const string DoneTag = "[DONE]";
        private const string DoneEvent = "done";

        /// <summary>
        /// https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events
        /// </summary>
        public static async Task<HttpResponseMessage> StreamEventsAsync(
            this OpenAIBaseEndpoint baseEndpoint,
            string uri,
            StringContent payload,
            Func<HttpResponseMessage, ServerSentEvent, Task> eventCallback,
            CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = payload;
            var response = await baseEndpoint.ServerSentEventStreamAsync(request, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(false, payload, cancellationToken: cancellationToken).ConfigureAwait(false);

            if (baseEndpoint.EnableDebug)
            {
                await response.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var events = new Stack<ServerSentEvent>();
            using var reader = new StreamReader(stream);

            try
            {
                while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } streamData)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!TryParseServerSentEventLine(streamData, out var type, out var value, out var data))
                    {
                        continue;
                    }

                    // if either value or data equals doneTag then stop processing events.
                    if (string.Equals(value, DoneTag, StringComparison.Ordinal) ||
                        string.Equals(value, DoneEvent, StringComparison.Ordinal) ||
                        string.Equals(data, DoneTag, StringComparison.Ordinal))
                    {
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

                    var hasInlineData = type != ServerSentEventKind.Data && !string.IsNullOrWhiteSpace(data);

                    if (hasInlineData)
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
                    else if (type == ServerSentEventKind.Data)
                    {
                        @event.Data = @event.Value;
                    }

                    if (type == ServerSentEventKind.Data && events.Count > 0 && events.Peek().Event == ServerSentEventKind.Event)
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
            finally
            {
                await response.CheckResponseAsync(baseEndpoint.EnableDebug, payload, null, events.Reverse().ToList(), cancellationToken).ConfigureAwait(false);
            }

            return response;
        }

        private static bool TryParseServerSentEventLine(string streamData, out ServerSentEventKind type, out string value, out string data)
        {
            type = ServerSentEventKind.Comment;
            value = string.Empty;
            data = string.Empty;

            if (string.IsNullOrWhiteSpace(streamData))
            {
                return false;
            }

            var span = streamData.AsSpan();
            var colonIndex = span.IndexOf(':');

            if (colonIndex < 0)
            {
                return false;
            }

            var typeSpan = TrimWhitespace(span[..colonIndex]);
            var valueSpan = span[(colonIndex + 1)..];

            while (!valueSpan.IsEmpty && valueSpan[0] == Space)
            {
                valueSpan = valueSpan[1..];
            }

            if (!valueSpan.IsEmpty && valueSpan[0] == Bom)
            {
                valueSpan = valueSpan[1..];
            }

            while (!valueSpan.IsEmpty && (valueSpan[^1] == Return || valueSpan[^1] == NewLine))
            {
                valueSpan = valueSpan[..^1];
            }

            value = valueSpan.Length == 0 ? string.Empty : new string(valueSpan);

            if (typeSpan.Length == 0)
            {
                return false;
            }

            type = ServerSentEvent.EventMap.GetValueOrDefault(new string(typeSpan), ServerSentEventKind.Comment);

            if (type == ServerSentEventKind.Data)
            {
                data = value;
            }

            return true;
        }

        private static ReadOnlySpan<char> TrimWhitespace(ReadOnlySpan<char> span)
        {
            var start = 0;
            var end = span.Length - 1;

            while (start <= end && char.IsWhiteSpace(span[start]))
            {
                start++;
            }

            while (end >= start && char.IsWhiteSpace(span[end]))
            {
                end--;
            }

            return start > end ? span[..0] : span[start..(end + 1)];
        }
    }
}
