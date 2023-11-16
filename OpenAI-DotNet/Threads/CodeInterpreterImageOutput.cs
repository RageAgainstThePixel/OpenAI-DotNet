using System.Text.Json.Serialization;

namespace OpenAI.Threads
{
    public sealed class CodeInterpreterImageOutput
    {
        /// <summary>
        /// The file ID of the image.
        /// </summary>
        [JsonPropertyName("file_id")]
        public string FileId { get; private set; }
    }
}