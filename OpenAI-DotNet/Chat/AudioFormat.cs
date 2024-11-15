// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Chat
{
    public enum AudioFormat
    {
        [EnumMember(Value = "pcm16")]
        Pcm16 = 1,
        [EnumMember(Value = "opus")]
        Opus,
        [EnumMember(Value = "mp3")]
        Mp3,
        [EnumMember(Value = "wav")]
        Wav,
        [EnumMember(Value = "flac")]
        Flac
    }
}
