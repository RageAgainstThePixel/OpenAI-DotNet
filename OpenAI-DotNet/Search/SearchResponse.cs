using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OpenAI
{
    /// <summary>
    /// Represents a response from the <see cref="SearchEndpoint"/>.
    /// </summary>
    internal sealed class SearchResponse : BaseResponse
    {
        /// <summary>
        /// The list of results
        /// </summary>
        [JsonPropertyName("data")]
        public List<SearchResult> Results { get; set; }
    }
}
