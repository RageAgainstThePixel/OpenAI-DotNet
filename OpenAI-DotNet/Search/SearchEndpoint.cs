using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI_DotNet
{
    /// <summary>
    /// The API lets you do semantic search over documents.
    /// This means that you can provide a query, such as a natural language question or a statement,
    /// and find documents that answer the question or are semantically related to the statement.
    /// The “documents” can be words, sentences, paragraphs or even longer documents.
    /// For example, if you provide documents "White House", "hospital", "school" and query "the president",
    /// you’ll get a different similarity score for each document. The higher the similarity score,
    /// the more semantically similar the document is to the query
    /// (in this example, “White House” will be most similar to “the president”).
    /// <see href="https://beta.openai.com/docs/api-reference/searches"/>
    /// </summary>
    public class SearchEndpoint : BaseEndPoint
    {
        /// <inheritdoc />
        internal SearchEndpoint(OpenAI api) : base(api) { }

        /// <inheritdoc />
        protected override string GetEndpoint(Engine engine = null)
        {
            return $"{Api.BaseUrl}engines/{engine?.EngineName ?? Api.DefaultEngine.EngineName}/search";
        }

        /// <summary>
        /// Perform a semantic search over a list of documents
        /// </summary>
        /// <param name="searchRequest">The request containing the query and the documents to match against</param>
        /// <param name="engine">Optional, <see cref="Engine"/> to use when calling the API.
        /// Defaults to <see cref="OpenAI.DefaultEngine"/>.</param>
        /// <returns>Asynchronously returns a Dictionary mapping each document to the score for that document.
        /// The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher),
        /// where a score above 200 usually means the document is semantically similar to the query.</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        private async Task<Dictionary<string, double>> GetSearchResultsAsync(SearchRequest searchRequest, Engine engine = null)
        {
            var jsonContent = JsonSerializer.Serialize(searchRequest, Api.JsonSerializationOptions);
            var response = await Api.Client.PostAsync(GetEndpoint(engine), jsonContent.ToJsonStringContent());

            if (response.IsSuccessStatusCode)
            {
                var resultAsString = await response.Content.ReadAsStringAsync();
                var searchResponse = JsonSerializer.Deserialize<SearchResponse>(resultAsString);

                if (searchResponse?.Results == null || searchResponse.Results.Count == 0)
                {
                    throw new HttpRequestException($"{nameof(GetSearchResultsAsync)} returned no results!  HTTP status code: {response.StatusCode}. Response body: {resultAsString}");
                }

                searchResponse.SetResponseData(response.Headers);

                return searchResponse.Results.ToDictionary(result => searchRequest.Documents[result.DocumentIndex], result => result.Score);
            }

            throw new HttpRequestException($"{nameof(GetSearchResultsAsync)} Failed!  HTTP status code: {response.StatusCode}. Request body: {jsonContent}");
        }

        /// <summary>
        /// Perform a semantic search of a query over a list of documents
        /// </summary>
        /// <param name="query">A query to match against</param>
        /// <param name="documents">Documents to search over, provided as a list of strings</param>
        /// <param name="engine">Optional, <see cref="Engine"/> to use when calling the API.
        /// Defaults to <see cref="OpenAI.DefaultEngine"/>.</param>
        /// <returns>Asynchronously returns a Dictionary mapping each document to the score for that document.
        /// The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher),
        /// where a score above 200 usually means the document is semantically similar to the query.</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task<Dictionary<string, double>> GetSearchResultsAsync(string query, IEnumerable<string> documents, Engine engine = null)
        {
            return await GetSearchResultsAsync(new SearchRequest(query, documents), engine);
        }

        /// <summary>
        /// Perform a semantic search of a query over a list of documents to get the single best match
        /// </summary>
        /// <param name="query">A query to match against</param>
        /// <param name="documents">Documents to search over, provided as a list of strings</param>
        /// <param name="engine">Optional, <see cref="Engine"/> to use when calling the API.
        /// Defaults to <see cref="OpenAI.DefaultEngine"/>.</param>
        /// <returns>Asynchronously returns the best matching document</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task<string> GetBestMatchAsync(string query, IEnumerable<string> documents, Engine engine = null)
        {
            var results = await GetSearchResultsAsync(new SearchRequest(query, documents), engine);
            return results.Count == 0 ? null : results.ToList().OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
        }

        /// <summary>
        /// Perform a semantic search of a query over a list of documents to get the single best match and its score
        /// </summary>
        /// <param name="query">A query to match against</param>
        /// <param name="documents">Documents to search over, provided as a list of strings</param>
        /// <param name="engine">Optional, <see cref="Engine"/> to use when calling the API.
        /// Defaults to <see cref="OpenAI.DefaultEngine"/>.</param>
        /// <returns>Asynchronously returns a tuple of the best matching document and its score.
        /// The similarity score is a positive score that usually ranges from 0 to 300 (but can sometimes go higher),
        /// where a score above 200 usually means the document is semantically similar to the query.</returns>
        /// <exception cref="HttpRequestException">Raised when the HTTP request fails</exception>
        public async Task<Tuple<string, double>> GetBestMatchWithScoreAsync(string query, IEnumerable<string> documents, Engine engine = null)
        {
            var results = await GetSearchResultsAsync(new SearchRequest(query, documents), engine);
            var (key, value) = results.ToList().OrderByDescending(kv => kv.Value).FirstOrDefault();
            return new Tuple<string, double>(key, value);
        }
    }
}
