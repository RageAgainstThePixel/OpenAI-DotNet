using System.Text.Json.Serialization;

namespace OpenAI.Assistants;

public sealed class CreateAssistantFileRequest
{
    /// <summary>
    /// A File ID (with purpose="assistants") that the assistant should use.
    /// Useful for tools like retrieval and code_interpreter that can access files.
    /// </summary>
    /// <returns></returns>
    [JsonPropertyName("file_id")]
    public string FileId { get; set; }
}