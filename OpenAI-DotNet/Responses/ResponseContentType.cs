// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Responses
{
    public enum ResponseContentType
    {
        [EnumMember(Value = "input_text")]
        InputText,
        [EnumMember(Value = "output_text")]
        OutputText,
        [EnumMember(Value = "input_audio")]
        InputAudio,
        [EnumMember(Value = "output_audio")]
        OutputAudio,
        [EnumMember(Value = "input_image")]
        InputImage,
        [EnumMember(Value = "input_file")]
        InputFile,
        [EnumMember(Value = "refusal")]
        Refusal,
    }
}
