// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Runtime.Serialization;

namespace OpenAI
{
    public enum ComparisonFilterType
    {
        [EnumMember(Value = "eq")]
        Equals = 1,
        [EnumMember(Value = "ne")]
        NotEquals,
        [EnumMember(Value = "gt")]
        GreaterThan,
        [EnumMember(Value = "gte")]
        GreaterThanOrEquals,
        [EnumMember(Value = "lt")]
        LessThan,
        [EnumMember(Value = "lte")]
        LessThanOrEquals,
    }
}
