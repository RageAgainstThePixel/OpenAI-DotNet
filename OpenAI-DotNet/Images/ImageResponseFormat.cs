// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Images
{
    public enum ImageResponseFormat
    {
        [EnumMember(Value = "b64_json")]
        B64_Json = 0,
        [EnumMember(Value = "url")]
        Url,
    }
}
