// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace OpenAI.Chat
{
    [Obsolete("Use ResponseFormat instead.")]
    public enum ChatResponseFormat
    {
        [EnumMember(Value = "text")]
        Text,
        [EnumMember(Value = "json_object")]
        Json
    }
}
