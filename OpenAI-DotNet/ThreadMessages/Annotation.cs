using System.Text.Json.Serialization;
using OpenAI.Extensions;

namespace OpenAI.Chat;

public class Annotation
{
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter<AnnotationType>))]
    public AnnotationType Type { get; set; }

    /// <summary>
    /// The text in the message content that needs to be replaced.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// A citation within the message that points to a specific quote from a specific File associated with the assistant or the message.
    /// Generated when the assistant uses the "retrieval" tool to search files.
    /// </summary>
    [JsonPropertyName("file_citation")]
    public FileCitation FileCitation { get; set; }

    /// <summary>
    /// A URL for the file that's generated when the assistant used the code_interpreter tool to generate a file.
    /// </summary>
    [JsonPropertyName("file_path")]
    public FilePath FilePath { get; set; }

    [JsonPropertyName("start_index")]
    public int StartIndex { get; set; }

    [JsonPropertyName("end_index")]
    public int EndIndex { get; set; }
}