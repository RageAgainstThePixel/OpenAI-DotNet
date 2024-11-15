// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Realtime
{
    public enum RealtimeContentType
    {
        [EnumMember(Value = "text")]
        Text,
        [EnumMember(Value = "audio")]
        Audio,
        [EnumMember(Value = "input_text")]
        InputText,
        [EnumMember(Value = "input_audio")]
        InputAudio
    }
}
