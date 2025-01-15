// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    [JsonConverter(typeof(AudioOutputConverter))]
    public sealed class AudioOutput
    {
        internal AudioOutput(string id, int? expiresAtUnixSeconds, Memory<byte> data, string transcript)
        {
            Id = id;
            this.data = data;
            Transcript = transcript;
            ExpiresAtUnixSeconds = expiresAtUnixSeconds;
        }

        public string Id { get; private set; }

        public string Transcript { get; private set; }

        private Memory<byte> data;

        public ReadOnlyMemory<byte> Data => data;

        public int? ExpiresAtUnixSeconds { get; private set; }

        public DateTime? ExpiresAt => ExpiresAtUnixSeconds.HasValue
            ? DateTimeOffset.FromUnixTimeSeconds(ExpiresAtUnixSeconds.Value).DateTime
            : null;

        public override string ToString() => Transcript ?? string.Empty;

        internal void AppendFrom(AudioOutput other)
        {
            if (other == null) { return; }

            if (!string.IsNullOrWhiteSpace(other.Id))
            {
                Id = other.Id;
            }

            if (other.ExpiresAtUnixSeconds.HasValue)
            {
                ExpiresAtUnixSeconds = other.ExpiresAtUnixSeconds;
            }

            if (!string.IsNullOrWhiteSpace(other.Transcript))
            {
                Transcript += other.Transcript;
            }

            if (other.Data.Length > 0)
            {
                data = data.ToArray().Concat(other.Data.ToArray()).ToArray();
            }
        }
    }
}
