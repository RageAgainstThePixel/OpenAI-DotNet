// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Responses
{
    public enum Truncation
    {
        [EnumMember(Value = "auto")]
        Auto = 1,
        [EnumMember(Value = "disabled")]
        Disabled,
    }
}
