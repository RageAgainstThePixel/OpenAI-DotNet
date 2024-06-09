// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class StreamOptions
    {
        [JsonPropertyName("include_usage")]
        public bool IncludeUsage { get; } = true;
    }
}
