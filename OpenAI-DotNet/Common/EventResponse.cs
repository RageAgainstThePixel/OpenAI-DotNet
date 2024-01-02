// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class EventResponse : BaseResponse
    {
        public EventResponse() { }

#pragma warning disable CS0618 // Type or member is obsolete
        internal EventResponse(Event @event)
        {
            Object = @event.Object;
            CreatedAtUnixTimeSeconds = @event.CreatedAtUnixTimeSeconds;
            Level = @event.Level;
            Message = @event.Message;
        }
#pragma warning restore CS0618 // Type or member is obsolete

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).DateTime;

        [JsonInclude]
        [JsonPropertyName("level")]
        public string Level { get; private set; }

        [JsonInclude]
        [JsonPropertyName("message")]
        public string Message { get; private set; }
    }
}