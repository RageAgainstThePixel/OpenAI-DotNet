using System.Runtime.Serialization;

namespace OpenAI
{
    public enum SortOrder
    {
        [EnumMember(Value = "desc")]
        Descending,
        [EnumMember(Value = "asc")]
        Ascending,
    }
}