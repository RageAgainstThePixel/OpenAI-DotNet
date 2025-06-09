// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class ComparisonFilter : IFilter
    {
        [JsonConstructor]
        public ComparisonFilter(string key, ComparisonFilterType type)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Type = type;
        }

        [JsonPropertyName("key")]
        public string Key { get; }

        [JsonPropertyName("type")]
        public ComparisonFilterType Type { get; }
    }
}
