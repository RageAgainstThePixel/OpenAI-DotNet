// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace OpenAI.Realtime
{
    [Flags]
    public enum RealtimeModality
    {
        [EnumMember(Value = "text")]
        Text = 1 << 0,
        [EnumMember(Value = "audio")]
        Audio = 1 << 1
    }
}
