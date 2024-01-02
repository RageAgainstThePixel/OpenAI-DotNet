// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Audio
{
    public enum SpeechResponseFormat
    {
        [EnumMember(Value = "mp3")]
        MP3,
        [EnumMember(Value = "opus")]
        Opus,
        [EnumMember(Value = "aac")]
        AAC,
        [EnumMember(Value = "flac")]
        Flac
    }
}
