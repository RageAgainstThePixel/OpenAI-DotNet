using System.Text.Json.Serialization;

namespace OpenAI.Chat
{
    public class FileCitation
    {
        /// <summary>
        /// The ID of the specific File the citation is from.
        /// </summary>
        [JsonPropertyName("file_id")]
        public string FileId { get; set; }
        
        /// <summary>
        /// The specific quote in the file.
        /// </summary>
        [JsonPropertyName("file_id")]
        public string Quote { get; set; }
    }
}