// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI
{
    public enum ContentType
    {
        [EnumMember(Value = "text")]
        Text,
        [EnumMember(Value = "image_url")]
        ImageUrl,
        [EnumMember(Value = "image_file")]
        ImageFile,
        [EnumMember(Value = "file")]
        File,
        [EnumMember(Value = "input_audio")]
        InputAudio,
        [EnumMember(Value = "refusal")]
        Refusal,
    }
}
