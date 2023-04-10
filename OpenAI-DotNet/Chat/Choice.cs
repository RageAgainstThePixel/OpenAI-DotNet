using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public sealed class Choice
    {
        [JsonInclude]
        [JsonPropertyName("message")]
        public Message Message { get; private set; }

        [JsonInclude]
        [JsonPropertyName("delta")]
        public Delta Delta { get; private set; }

        [JsonInclude]
        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; private set; }

        [JsonInclude]
        [JsonPropertyName("index")]
        public int Index { get; private set; }

        public override string ToString() => Message?.Content ?? Delta.Content;

        public static implicit operator string(Choice choice) => choice.ToString();
    }
}
