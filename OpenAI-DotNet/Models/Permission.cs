using System;
using System.Text.Json.Serialization;

namespace OpenAI.Models
{
    public sealed class Permission
    {
        [JsonConstructor]
        public Permission(string id, string @object, int createdAtUnixTime, bool allowCreateEngine, bool allowSampling, bool allowLogprobs, bool allowSearchIndices, bool allowView, bool allowFineTuning, string organization, object group, bool isBlocking)
        {
            Id = id;
            Object = @object;
            CreatedAtUnixTime = createdAtUnixTime;
            AllowCreateEngine = allowCreateEngine;
            AllowSampling = allowSampling;
            AllowLogprobs = allowLogprobs;
            AllowSearchIndices = allowSearchIndices;
            AllowView = allowView;
            AllowFineTuning = allowFineTuning;
            Organization = organization;
            Group = group;
            IsBlocking = isBlocking;
        }

        [JsonPropertyName("id")]
        public string Id { get; }

        [JsonPropertyName("object")]
        public string Object { get; }

        [JsonPropertyName("created")]
        public int CreatedAtUnixTime { get; }

        [JsonIgnore]
        public DateTime CreatedAt => DateTimeOffset.FromUnixTimeSeconds(CreatedAtUnixTime).DateTime;

        [JsonPropertyName("allow_create_engine")]
        public bool AllowCreateEngine { get; }

        [JsonPropertyName("allow_sampling")]
        public bool AllowSampling { get; }

        [JsonPropertyName("allow_logprobs")]
        public bool AllowLogprobs { get; }

        [JsonPropertyName("allow_search_indices")]
        public bool AllowSearchIndices { get; }

        [JsonPropertyName("allow_view")]
        public bool AllowView { get; }

        [JsonPropertyName("allow_fine_tuning")]
        public bool AllowFineTuning { get; }

        [JsonPropertyName("organization")]
        public string Organization { get; }

        [JsonPropertyName("group")]
        public object Group { get; }

        [JsonPropertyName("is_blocking")]
        public bool IsBlocking { get; }
    }
}
