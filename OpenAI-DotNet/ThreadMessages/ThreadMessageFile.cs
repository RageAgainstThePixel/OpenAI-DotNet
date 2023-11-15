using System.Text.Json.Serialization;

namespace OpenAI.ThreadMessages;

public class ThreadMessageFile
{
    /// <summary>
    /// The identifier, which can be referenced in API endpoints.
    /// </summary>
    /// <returns></returns>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The object type, which is always thread.message.file.
    /// </summary>
    /// <returns></returns>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "thread.message.file";

    /// <summary>
    /// The Unix timestamp (in seconds) for when the message file was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }

    /// <summary>
    /// The ID of the message that the File is attached to.
    /// </summary>
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; }
}