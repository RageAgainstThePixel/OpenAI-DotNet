// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Responses
{
    public enum MCPToolRequireApproval
    {
        [EnumMember(Value = "never")]
        Never,
        [EnumMember(Value = "always")]
        Always
    }
}
