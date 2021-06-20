using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// Represents a single search result in <see cref="SearchResponse"/>.
    /// </summary>
    internal sealed class SearchResult
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