using NUnit.Framework;
using OpenAI;
using System;
using System.Threading.Tasks;

namespace OpenAI_Tests
{
    public class SearchEndpointTests
    {
        private readonly string query = "Washington DC";
        private readonly string[] documents = { "Canada", "China", "USA", "Spain" };

        [Test]
        public async Task GetSearchResults()
        {
            var api = new OpenAIClient();

            Assert.IsNotNull(api.SearchEndpoint);

            var results = await api.SearchEndpoint.GetSearchResultsAsync(query, documents, Engine.Curie);
            Assert.IsNotNull(results);
            Assert.IsNotEmpty(results);
        }

        [Test]
        public async Task GetBestMatch()
        {
            var api = new OpenAIClient();

            Assert.IsNotNull(api.SearchEndpoint);

            var result = await api.SearchEndpoint.GetBestMatchAsync(query, documents, Engine.Curie);
            Assert.IsNotNull(result);
            Assert.AreEqual("USA", result);
        }

        [Test]
        public async Task GetBestMatchWithScore()
        {
            var api = new OpenAIClient();

            Assert.IsNotNull(api.SearchEndpoint);

            var result = await api.SearchEndpoint.GetBestMatchWithScoreAsync(query, documents, Engine.Curie);
            Assert.IsNotNull(result);
            var (match, score) = result;
            Assert.AreEqual("USA", match);
            Assert.NotZero(score);
        }
    }
}