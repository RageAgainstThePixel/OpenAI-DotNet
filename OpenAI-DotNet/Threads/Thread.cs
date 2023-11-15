using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI.Threads;

public sealed class Thread
{
    /// <summary>
    /// The identifier, which can be referenced in API endpoints.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The object type, which is always thread.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; set; } = "thread";

    /// <summary>
    /// The Unix timestamp (in seconds) for when the thread was created.
    /// </summary>
    /// <returns></returns>
    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }

    /// <summary>
    /// Set of 16 key-value pairs that can be attached to an object.
    /// This can be useful for storing additional information about the object in a structured format.
    /// Keys can be a maximum of 64 characters long and values can be a maxium of 512 characters long.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string> Metadata { get; set; }
}