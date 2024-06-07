// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
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

        private static JsonSerializerOptions sseJsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        /// <summary>
        /// https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events/Using_server-sent_events
        /// </summary>
        public static async Task<HttpResponseMessage> StreamEventsAsync(this OpenAIBaseEndpoint baseEndpoint, string endpoint, StringContent payload, Action<HttpResponseMessage, string> eventCallback, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = payload;
            var response = await baseEndpoint.HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            await response.CheckResponseAsync(false, payload, null, cancellationToken).ConfigureAwait(false);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var responseStream = baseEndpoint.EnableDebug ? new MemoryStream() : null;
            var events = new Stack<Dictionary<string, object>>();
            using var reader = new StreamReader(stream);
            var isEndOfStream = false;

            try
            {
                while (await reader.ReadLineAsync() is { } streamData)
                {
                    if (isEndOfStream) { break; }
                    cancellationToken.ThrowIfCancellationRequested();
                    if (string.IsNullOrWhiteSpace(streamData)) { continue; }

                    var matches = sseRegex.Matches(streamData);

                    for (var i = 0; i < matches.Count; i++)
                    {
                        Match match = matches[i];
                        string type;
                        string value;
                        string data;

                        const string comment = nameof(comment);
                        type = match.Groups[nameof(type)].Value.Trim();
                        // If the field type is not provided, treat it as a comment
                        type = string.IsNullOrEmpty(type) ? comment : type;
                        value = match.Groups[nameof(value)].Value.Trim();
                        data = match.Groups[nameof(data)].Value.Trim();

                        if ((type.Equals("event") && value.Equals("done") && data.Equals("[DONE]")) ||
                            (type.Equals("data") && value.Equals("[DONE]")))
                        {
                            isEndOfStream = true;
                            break;
                        }

                        var eventObject = new Dictionary<string, object>();

                        try
                        {
                            eventObject[type] = JsonNode.Parse(value);
                        }
                        catch
                        {
                            eventObject[type] = value;
                        }

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            try
                            {
                                eventObject[nameof(data)] = JsonNode.Parse(data);
                            }
                            catch
                            {
                                eventObject[nameof(data)] = data;
                            }
                        }

                        if (type.Equals("data") && events.Count > 0 && events.Peek().ContainsKey("event"))
                        {
                            var previousEvent = events.Pop();
                            previousEvent[nameof(data)] = eventObject[type];
                            var eventData = JsonSerializer.Serialize(previousEvent, sseJsonOptions);
                            eventCallback?.Invoke(response, eventData);
                            Console.WriteLine(eventData);
                            events.Push(previousEvent);
                        }
                        else
                        {
                            if (!type.Equals("event"))
                            {
                                var eventData = JsonSerializer.Serialize(eventObject, sseJsonOptions);
                                eventCallback?.Invoke(response, eventData);
                                Console.WriteLine(eventData);
                            }

                            events.Push(eventObject);
                        }
                    }
                }
            }
            finally
            {
                if (responseStream != null)
                {
                    var orderedEvents = new List<Dictionary<string, object>>(events);
                    orderedEvents.Reverse();
                    await responseStream.WriteAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderedEvents)), cancellationToken);
                }
            }

            await response.CheckResponseAsync(baseEndpoint.EnableDebug, payload, responseStream, cancellationToken).ConfigureAwait(false);
            return response;
        }
    }
}
