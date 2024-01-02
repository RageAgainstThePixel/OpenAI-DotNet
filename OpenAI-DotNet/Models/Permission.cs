// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.Models
{
    public sealed class Permission
    {
        [JsonInclude]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonInclude]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        [JsonInclude]
        [JsonPropertyName("created")]
        public int CreatedAtUnitTimeSeconds { get; private set; }

        [Obsolete("use CreatedAtUnitTimeSeconds")]
        public int CreatedAtUnixTime => CreatedAtUnitTimeSeconds;

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnitTimeSeconds).DateTime;

        [JsonInclude]
        [JsonPropertyName("allow_create_engine")]
        public bool AllowCreateEngine { get; private set; }

        [JsonInclude]
        [JsonPropertyName("allow_sampling")]
        public bool AllowSampling { get; private set; }

        [JsonInclude]
        [JsonPropertyName("allow_logprobs")]
        public bool AllowLogprobs { get; private set; }

        [JsonInclude]
        [JsonPropertyName("allow_search_indices")]
        public bool AllowSearchIndices { get; private set; }

        [JsonInclude]
        [JsonPropertyName("allow_view")]
        public bool AllowView { get; private set; }

        [JsonInclude]
        [JsonPropertyName("allow_fine_tuning")]
        public bool AllowFineTuning { get; private set; }

        [JsonInclude]
        [JsonPropertyName("organization")]
        public string Organization { get; private set; }

        [JsonInclude]
        [JsonPropertyName("group")]
        public object Group { get; private set; }

        [JsonInclude]
        [JsonPropertyName("is_blocking")]
        public bool IsBlocking { get; private set; }
    }
}
