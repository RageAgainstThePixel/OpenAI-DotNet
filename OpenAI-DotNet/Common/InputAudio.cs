// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class InputAudio
    {
        public InputAudio() { }

        public InputAudio(ReadOnlyMemory<byte> memory, InputAudioFormat format)
            : this(memory.Span, format)
        {
        }

        public InputAudio(ReadOnlySpan<byte> span, InputAudioFormat format)
            : this($"data:audio/{format};base64,{Convert.ToBase64String(span)}", format)
        {
        }

        public InputAudio(byte[] data, InputAudioFormat format)
            : this($"data:audio/{format};base64,{Convert.ToBase64String(data)}", format)
        {
        }

        public InputAudio(string data, InputAudioFormat format)
        {
            Data = data;
            Format = format;
        }

        [JsonInclude]
        [JsonPropertyName("data")]
        public string Data { get; private set; }

        [JsonInclude]
        [JsonPropertyName("format")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InputAudioFormat Format { get; private set; }

        public override string ToString() => Data;

        public void AppendFrom(InputAudio other)
        {
            if (other == null) { return; }

            if (other.Format > 0)
            {
                Format = other.Format;
            }

            if (!string.IsNullOrWhiteSpace(other.Data))
            {
                Data += other.Data;
            }
        }
    }
}
