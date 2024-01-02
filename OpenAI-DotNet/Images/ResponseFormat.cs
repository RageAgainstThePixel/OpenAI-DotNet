// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Images
{
    public enum ResponseFormat
    {
        [EnumMember(Value = "url")]
        Url,
        [EnumMember(Value = "b64_json")]
        B64_Json
    }
}