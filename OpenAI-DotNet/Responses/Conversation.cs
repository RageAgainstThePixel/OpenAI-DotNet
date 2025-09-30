// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class Conversation
    {
        [JsonInclude]
        [JsonPropertyName("created_at")]
        public long CreatedAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTimeSeconds).UtcDateTime;

        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("metadata")]
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        public override string ToString() => Id;

        public static implicit operator string(Conversation conversation) => conversation?.Id;
    }
}
