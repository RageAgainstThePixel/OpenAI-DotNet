using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Choice
    {
        public Choice() { }

        public Choice(
            Message message,
            Delta delta,
            string finishReason,
            int index)
            : this()
        {
            Message = message;
            Delta = delta;
            FinishReason = finishReason;
            Index = index;
        }

        [JsonInclude]
        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Message Message { get; private set; }

        [JsonInclude]
        [JsonPropertyName("delta")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Delta Delta { get; private set; }

        [JsonInclude]
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; private set; }

        [JsonInclude]
        [JsonPropertyName("index")]
        public int Index { get; private set; }

        public override string ToString() => Message?.ToString() ?? Delta?.ToString() ?? string.Empty;

        public static implicit operator string(Choice choice) => choice.ToString();
    }
}
