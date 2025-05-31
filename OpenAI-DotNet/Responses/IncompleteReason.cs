// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI.Responses
{
    public enum IncompleteReason
    {
        None = 0,
        [EnumMember(Value = "content_filter")]
        ContentFilter,
        [EnumMember(Value = "max_output_tokens")]
        MaxOutputTokens
    }
}
