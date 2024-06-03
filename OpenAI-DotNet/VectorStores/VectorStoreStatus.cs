// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.VectorStores
{
    public enum VectorStoreStatus
    {
        NotStarted = 0,
        [EnumMember(Value = "in_progress")]
        InProgress,
        [EnumMember(Value = "completed")]
        Completed,
        [EnumMember(Value = "failed")]
        Expired
    }
}
