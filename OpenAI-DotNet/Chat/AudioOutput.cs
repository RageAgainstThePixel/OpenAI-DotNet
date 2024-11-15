// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    [JsonConverter(typeof(AudioOutputConverter))]
    public sealed class AudioOutput
    {
        internal AudioOutput(string id, int expiresAtUnixSeconds, ReadOnlyMemory<byte> data, string transcript)
        {
            Id = id;
            ExpiresAtUnixSeconds = expiresAtUnixSeconds;
            Data = data;
            Transcript = transcript;
        }

        public string Id { get; }

        public int ExpiresAtUnixSeconds { get; }

        public DateTime ExpiresAt => DateTimeOffset.FromUnixTimeSeconds(ExpiresAtUnixSeconds).DateTime;

        public ReadOnlyMemory<byte> Data { get; }

        public string Transcript { get; }

        public override string ToString() => Transcript ?? string.Empty;
    }
}
