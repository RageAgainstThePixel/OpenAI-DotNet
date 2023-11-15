using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.ThreadMessages;

public class CodeInterpreter
{
    /// <summary>
    /// The input to the Code Interpreter tool call.
    /// </summary>
    [JsonPropertyName("input")]
    public string Input { get; set; }
    
    /// <summary>
    /// The outputs from the Code Interpreter tool call.
    /// Code Interpreter can output one or more items, including text (logs) or images (image).
    /// Each of these are represented by a different object type.
    /// </summary>
    [JsonPropertyName("outputs")]
    public IReadOnlyList<CodeInterpreterOutputs> Outputs { get; set; }
}