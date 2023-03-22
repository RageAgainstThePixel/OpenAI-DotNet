using System.Text.Json.Serialization;

namespace OpenAI.Edits
{
    public sealed class Choice
    {
        [JsonInclude]
        [JsonPropertyName("text")]
        public string Text { get; private set; }

        [JsonInclude]
        [JsonPropertyName("index")]
        public int Index { get; private set; }

        /// <summary>
        /// Gets the main text of this completion
        /// </summary>
        public override string ToString() => Text;

        public static implicit operator string(Choice choice) => choice.Text;
    }
}
