using System.Text.Json.Serialization;

namespace OpenAI.ThreadMessages;

public sealed class RunStepMessageCreation
{
    /// <summary>
    /// The ID of the message that was created by this run step.
    /// </summary>
    [JsonPropertyName("message_id")]
    public string MessageId { get; set; }
}