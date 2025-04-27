// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Realtime
{
    public enum TurnDetectionType
    {
        Disabled = 0,
        [EnumMember(Value = "server_vad")]
        Server_VAD,
        [EnumMember(Value = "semantic_vad")]
        Semantic_VAD,
    }
}
