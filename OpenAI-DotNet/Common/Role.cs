// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace OpenAI
{
    public enum Role
    {
        [EnumMember(Value = "system")]
        System = 1,
        [EnumMember(Value = "developer")]
        Developer,
        [EnumMember(Value = "assistant")]
        Assistant,
        [EnumMember(Value = "user")]
        User,
        [Obsolete("Use Tool")]
        [EnumMember(Value = "function")]
        Function,
        [EnumMember(Value = "tool")]
        Tool
    }
}
