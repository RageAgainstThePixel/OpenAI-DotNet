using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Choice
    {
        [JsonConstructor]
        public Choice(
            Message message,
            string finishReason,
            int index)
        {
            Message = message;
            FinishReason = finishReason;
            Index = index;
        }

        [JsonPropertyName("message")]
        public Message Message { get; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; }

        [JsonPropertyName("index")]
        public int Index { get; }

        public override string ToString() => Message.ToString();

        public static implicit operator string(Choice choice) => choice.ToString();
    }
}
