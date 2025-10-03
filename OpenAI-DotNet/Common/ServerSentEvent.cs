// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ServerSentEvent : IServerSentEvent
    {
        internal static readonly IReadOnlyDictionary<string, ServerSentEventKind> EventMap = new Dictionary<string, ServerSentEventKind>(StringComparer.OrdinalIgnoreCase)
        {
            { "comment", ServerSentEventKind.Comment },
            { "event", ServerSentEventKind.Event },
            { "data", ServerSentEventKind.Data },
            { "id", ServerSentEventKind.Id },
            { "retry", ServerSentEventKind.Retry }
        };

        internal ServerSentEvent(ServerSentEventKind @event) => Event = @event;

        [JsonInclude]
        public ServerSentEventKind Event { get; }

        [JsonInclude]
        public JsonNode Value { get; internal set; }

        [JsonInclude]
        public JsonNode Data { get; internal set; }

        [JsonIgnore]
        public string Object => "stream.event";

        public override string ToString()
            => ToJsonString();

        public string ToJsonString()
        {
            var @event = new JsonObject
            {
                { Event.ToString().ToLower(), Value.DeepClone() }
            };

            if (Data != null)
            {
                @event.Add(nameof(ServerSentEventKind.Data).ToLower(), Data.DeepClone());
            }

            return @event.ToEscapedJsonString();
        }
    }
}
