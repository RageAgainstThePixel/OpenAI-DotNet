using System;
using NUnit.Framework;
using OpenAI_DotNet;

namespace OpenAI_Tests
{
    public class SearchEndpointTests
    {
        [SetUp]
        public void Setup()
        {
            Authentication.Default = new Authentication(Environment.GetEnvironmentVariable("TEST_OPENAI_SECRET_KEY"));
        }

        private readonly string query = "Washington DC";
        private readonly string[] documents = { "Canada", "China", "USA", "Spain" };

        [Test]
        public void GetSearchResults()
        {
            var api = new OpenAI(Engine.Curie);

            Assert.IsNotNull(api.SearchEndpoint);

            var results = api.SearchEndpoint.GetSearchResultsAsync(query, documents).Result;
            Assert.IsNotNull(results);
            Assert.IsNotEmpty(results);
        }

        [Test]
        public void GetBestMatch()
        {
            var api = new OpenAI(Engine.Curie);

            Assert.IsNotNull(api.SearchEndpoint);

            var result = api.SearchEndpoint.GetBestMatchAsync(query, documents).Result;
            Assert.IsNotNull(result);
            Assert.AreEqual("USA", result);
        }

        [Test]
        public void GetBestMatchWithScore()
        {
            var api = new OpenAI(Engine.Curie);

            Assert.IsNotNull(api.SearchEndpoint);

            var result = api.SearchEndpoint.GetBestMatchWithScoreAsync(query, documents).Result;
            Assert.IsNotNull(result);
            var (match, score) = result;
            Assert.AreEqual("USA", match);
            Assert.NotZero(score);
        }
    }
}