using System.Text.Json.Serialization;

namespace OpenAI.ThreadRuns;

public sealed class ToolCall
{
    /// <summary>
    /// The ID of the tool call. This ID must be referenced when you submit the tool outputs in using the Submit tool outputs to run endpoint.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    /// <summary>
    /// The type of tool call the output is required for. For now, this is always function.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// The function definition.
    /// </summary>
    [JsonPropertyName("function")]
    public ToolCallFunction Function { get; set; }
}