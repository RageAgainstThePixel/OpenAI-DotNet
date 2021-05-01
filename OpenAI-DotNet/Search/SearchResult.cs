using System.Text.Json.Serialization;

namespace OpenAI_DotNet
{
    /// <summary>
    /// Used internally to deserialize a result from the Document Search API
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// The index of the document as originally supplied
        /// </summary>
        [JsonPropertyName("document")]
        public int DocumentIndex { get; set; }

        /// <summary>
        /// The relative score of this document
        /// </summary>
        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}