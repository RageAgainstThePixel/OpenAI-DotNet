using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Reasoning
    {
        public static implicit operator Reasoning(ReasoningEffort effort) => new(effort);

        public Reasoning() { }

        public Reasoning(ReasoningEffort effort, ReasoningSummary summary = ReasoningSummary.Auto)
        {
            Effort = effort;
            Summary = summary;
        }

        /// <summary>
        /// Constrains effort on reasoning for reasoning models.
        /// Currently supported values are: Low, Medium, High.
        /// Reducing reasoning effort can result in faster responses and fewer tokens used on reasoning in a response.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("effort")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ReasoningEffort Effort { get; private set; }

        /// <summary>
        /// A summary of the reasoning performed by the model.
        /// This can be useful for debugging and understanding the model's reasoning process.
        /// One of `auto`, `concise`, or `detailed`.
        /// </summary>
        [JsonInclude]
        [JsonPropertyName("summary")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ReasoningSummary Summary { get; private set; }
    }
}
