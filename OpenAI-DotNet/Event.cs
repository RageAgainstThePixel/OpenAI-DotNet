using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Event
    {
        [JsonConstructor]
        public Event(
            string @object,
            int createdAtUnixTime,
            string level,
            string message
        )
        {
            Object = @object;
            CreatedAtUnixTime = createdAtUnixTime;
            Level = level;
            Message = message;
        }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTime { get; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTime).DateTime;

        [JsonPropertyName("level")]
        public string Level { get; }

        [JsonPropertyName("message")]
        public string Message { get; }
    }
}
