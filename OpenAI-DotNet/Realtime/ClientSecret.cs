// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ClientSecret
    {
        public ClientSecret() { }

        public ClientSecret(int? expiresAfter = null)
        {
            ExpiresAfter = expiresAfter ?? 600;
        }

        [JsonInclude]
        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string EphemeralApiKey { get; private set; }

        [JsonInclude]
        [JsonPropertyName("expires_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? ExpiresAtUnixTimeSeconds { get; private set; }

        [JsonIgnore]
        public DateTime? ExpiresAt
            => ExpiresAtUnixTimeSeconds.HasValue
                ? DateTimeOffset.FromUnixTimeSeconds(ExpiresAtUnixTimeSeconds.Value).UtcDateTime
                : null;

        [JsonInclude]
        [JsonPropertyName("expires_after")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ExpiresAfter ExpiresAfter { get; private set; }

        public static implicit operator ClientSecret(int? expiresAfter) => new(expiresAfter);
    }
}
