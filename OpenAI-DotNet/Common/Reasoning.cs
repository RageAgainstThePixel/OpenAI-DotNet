using OpenAI.Responses;
using System.Text.Json.Serialization;

namespace OpenAI
{
    public sealed class Reasoning : IResponseItem
    {
        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public ResponseItemType Type { get; private set; } = ResponseItemType.WebSearchCall;

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("object")]
        public string Object { get; private set; }

        /// <inheritdoc />
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [JsonPropertyName("status")]
        public ResponseStatus Status { get; private set; }

        /// <summary>
        /// Constrains effort on reasoning for reasoning models.
        /// Currently supported values are: Low, Medium, High.
        /// Reducing reasoning effort can result in faster responses and fewer tokens used on reasoning in a response.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ReasoningEffort Effort { get; private set; }

        /// <summary>
        /// A summary of the reasoning performed by the model.
        /// This can be useful for debugging and understanding the model's reasoning process.
        /// One of `auto`, `concise`, or `detailed`.
        /// </summary>
        [JsonInclude]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ReasoningSummary Summary { get; private set; }
    }
}
