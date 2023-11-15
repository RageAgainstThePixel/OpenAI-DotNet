using System.Text.Json.Serialization;

namespace OpenAI.ThreadRuns;

public sealed class ToolCallFunction
{
    /// <summary>
    /// The name of the function.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    /// <summary>
    /// The arguments that the model expects you to pass to the function.
    /// </summary>
    [JsonPropertyName("arguments")]
    public string Arguments { get; set; }
}