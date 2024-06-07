// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace OpenAI.Extensions
{
    public sealed class ServerSentEvent
    {
        internal static readonly Dictionary<string, ServerSentEventKind> EventMap = new()
        {
            { "comment", ServerSentEventKind.Comment },
            { "event", ServerSentEventKind.Event },
            { "data", ServerSentEventKind.Data },
            { "id", ServerSentEventKind.Id },
            { "retry", ServerSentEventKind.Retry },
        };

        internal ServerSentEvent(ServerSentEventKind @event) => Event = @event;

        public ServerSentEventKind Event { get; }

        public JsonNode Value { get; internal set; }

        public JsonNode Data { get; internal set; }

        public override string ToString()
        {
            var @event = new JsonObject
            {
                {
                    Event.ToString().ToLower(), Value
                }
            };

            if (Data != null)
            {
                @event.Add(ServerSentEventKind.Data.ToString().ToLower(), Data);
            }

            return @event.ToJsonString(ResponseExtensions.DebugJsonOptions);
        }
    }
}
