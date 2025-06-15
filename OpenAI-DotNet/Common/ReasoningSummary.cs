using System.Runtime.Serialization;

namespace OpenAI
{
    /// <summary>
    /// A summary of the reasoning performed by the model.
    /// This can be useful for debugging and understanding the model's reasoning process.
    /// One of `auto`, `concise`, or `detailed`.
    /// </summary>
    public enum ReasoningSummary
    {
        [EnumMember(Value = "auto")]
        Auto = 1,
        [EnumMember(Value = "concise")]
        Concise,
        [EnumMember(Value = "detailed")]
        Detailed
    }
}