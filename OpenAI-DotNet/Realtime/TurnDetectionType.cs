// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Realtime
{
    public enum TurnDetectionType
    {
        Disabled,
        [EnumMember(Value = "server_vad")]
        Server_VAD,
    }
}
