// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Text.Json.Serialization;

namespace OpenAI.Realtime
{
    public sealed class ExpiresAfter
    {
        public ExpiresAfter() { }

        public ExpiresAfter(int seconds = 600)
            => Seconds = seconds;

        [JsonInclude]
        [JsonPropertyName("anchor")]
        public string Anchor { get; private set; } = "created_at";

        [JsonInclude]
        [JsonPropertyName("seconds")]
        public int Seconds { get; private set; } = 600;

        public static implicit operator ExpiresAfter(int seconds)
            => new(seconds);
    }
}
