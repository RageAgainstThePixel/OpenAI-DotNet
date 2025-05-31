// Licensed under the MIT License. See LICENSE in the project root for license information.

using OpenAI.Extensions;
using System;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    public sealed class AudioContent : IResponseContent, IAppendable<AudioContent>
    {
        public AudioContent() { }

        public AudioContent(byte[] data, InputAudioFormat format)
            : this($"data:audio/{format};base64,{Convert.ToBase64String(data)}", format)
        {
        }

        public AudioContent(string data, InputAudioFormat format)
        {
            Data = data;
            Format = format;
            Type = ResponseContentType.InputAudio;
        }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("index")]
        public int? Index { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseContentType Type { get; private set; } = ResponseContentType.InputAudio;

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("data")]
        public string Data { get; private set; }

        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("format")]
        public InputAudioFormat Format { get; private set; }

        public override string ToString() => Data;

        public void AppendFrom(AudioContent other)
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
