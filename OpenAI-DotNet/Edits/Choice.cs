using System.Text.Json.Serialization;

namespace OpenAI.Edits
{
    public sealed class Choice
    {
        [JsonConstructor]
        public Choice(string text, int index)
        {
            Text = text;
            Index = index;
        }

        [JsonPropertyName("text")]
        public string Text { get; }

        [JsonPropertyName("index")]
        public int Index { get; }

        /// <summary>
        /// Gets the main text of this completion
        /// </summary>
        public override string ToString() => Text;

        public static implicit operator string(Choice choice) => choice.Text;
    }
}
