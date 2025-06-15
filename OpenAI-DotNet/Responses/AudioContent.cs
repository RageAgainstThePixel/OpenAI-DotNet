// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI.Responses
{
    [JsonConverter(typeof(AudioContentConverter))]
    public sealed class AudioContent : BaseResponse, IResponseContent
    {
        public AudioContent(ReadOnlyMemory<byte> memory, InputAudioFormat format)
            : this(memory.Span, format)
        {
        }

        public AudioContent(ReadOnlySpan<byte> span, InputAudioFormat format)
        : this($"data:audio/{format};base64,{Convert.ToBase64String(span)}", format)
        {
        }

        public AudioContent(byte[] bytes, InputAudioFormat format)
            : this($"data:audio/{format};base64,{Convert.ToBase64String(bytes)}", format)
        {
        }

        public AudioContent(string base64Data, InputAudioFormat format)
        {
            Base64Data = base64Data;
            Format = format;
            Type = ResponseContentType.InputAudio;
        }

        internal AudioContent(ResponseContentType type, string base64Data = null, InputAudioFormat format = 0, string transcript = null)
        {
            Type = type;
            Base64Data = base64Data;

            if (!string.IsNullOrWhiteSpace(base64Data))
            {
                data = Convert.FromBase64String(base64Data);
            }

            Format = format;
            Transcript = transcript;
        }

        [JsonPropertyName("type")]
        public ResponseContentType Type { get; private set; }

        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Base64Data { get; private set; }

        private Memory<byte> data = Memory<byte>.Empty;

        [JsonIgnore]
        public ReadOnlyMemory<byte> Data => data;

        [JsonPropertyName("format")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public InputAudioFormat Format { get; private set; }

        [JsonPropertyName("transcript")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Transcript { get; private set; }

        [JsonIgnore]
        public string Object => Type.ToString();

        internal void AppendFrom(AudioContent other)
        {
            if (other == null)
            {
                return;
            }

            if (other.Type > 0)
            {
                Type = other.Type;
            }

            if (!string.IsNullOrWhiteSpace(other.Base64Data))
            {
                Base64Data += other.Base64Data;
            }

            if (other.Data.Length > 0)
            {
                data = data.ToArray().Concat(other.Data.ToArray()).ToArray();
            }

            if (other.Format > 0)
            {
                Format = other.Format;
            }

            if (!string.IsNullOrWhiteSpace(other.Transcript))
            {
                Transcript += other.Transcript;
            }
        }
    }
}
