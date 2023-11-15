using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.ThreadRuns;

public sealed class RunStepToolCall
{
    /// <summary>
    /// The ID of the tool call.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The type of tool call.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<RunStepToolCallType>))]
    public RunStepToolCallType Type { get; set; }

    /// <summary>
    /// The Code Interpreter tool call definition.
    /// </summary>
    [JsonPropertyName("code_interpreter")]
    public CodeInterpreter CodeInterpreter { get; set; }
}