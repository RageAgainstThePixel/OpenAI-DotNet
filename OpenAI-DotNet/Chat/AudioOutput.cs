// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class AudioOutput
    {
        // TODO set data
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonInclude]
        [JsonIgnore]
        public int ExpiresAtUnixSeconds { get; }

        [JsonInclude]
        [JsonIgnore]
        public DateTime ExpiresAt => DateTimeOffset.FromUnixTimeSeconds(ExpiresAtUnixSeconds).DateTime;

        [JsonInclude]
        [JsonIgnore]
        public string Data { get; }

        [JsonInclude]
        [JsonIgnore]
        public string Transcript { get; }

        public override string ToString() => Transcript ?? string.Empty;
    }
}
