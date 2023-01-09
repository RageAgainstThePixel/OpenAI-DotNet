using System.Text.Json.Serialization;

namespace OpenAI.Edits;

public sealed class Choice
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>
    /// Gets the main text of this completion
    /// </summary>
    public override string ToString() => Text;

    public static implicit operator string(Choice choice) => choice.Text;
}