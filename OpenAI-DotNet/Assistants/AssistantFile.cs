using System.Text.Json.Serialization;

namespace OpenAI.Assistants;

/// <summary>
/// File attached to an assistant.
/// </summary>
public sealed class AssistantFile
{
    /// <summary>
    /// The identifier, which can be referenced in API endpoints.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The object type, which is always assistant.file.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "assistant.file";
        
    /// <summary>
    /// The Unix timestamp (in seconds) for when the assistant file was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }
        
    /// <summary>
    /// The assistant ID that the file is attached to.
    /// </summary>
    /// <returns></returns>
    [JsonPropertyName("assistant_id")]
    public string AssistantId { get; set; }
}