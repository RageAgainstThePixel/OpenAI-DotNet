// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI
{
    public enum CompoundFilterOperator
    {
        [EnumMember(Value = "and")]
        And = 1,
        [EnumMember(Value = "or")]
        Or = 2,
    }
}
