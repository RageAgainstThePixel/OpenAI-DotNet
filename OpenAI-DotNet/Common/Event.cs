using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Event
    {
        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created_at")]
        public int CreatedAtUnixTimeSeconds { get; private set; }

        [Obsolete("use CreatedAtUnixTimeSeconds")]
        public int CreatedAtUnixTime => CreatedAtUnixTimeSeconds;

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
