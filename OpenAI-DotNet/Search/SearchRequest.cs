using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace OpenAI_DotNet
{
    internal class SearchRequest
    {
        [JsonPropertyName("query")]
        public string Query { get; }

        [JsonPropertyName("documents")]
        public IReadOnlyList<string> Documents { get; }

        public SearchRequest(string query, IEnumerable<string> documents)
        {
            Query = query;
            Documents = documents?.ToList() ?? new List<string>();
        }
    }
}
