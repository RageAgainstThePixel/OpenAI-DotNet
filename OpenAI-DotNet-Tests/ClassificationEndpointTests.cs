using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenAI;

namespace OpenAI_Tests
{
    public class ClassificationEndpointTests
    {
        private readonly string query = "It is a raining day :(";
        private readonly string[] labels = { "Positive", "Negative", "Neutral" };
        private readonly Dictionary<string, string> examples = new Dictionary<string, string>
        {
            { "A happy moment", "Positive" },
            { "I am sad.", "Negative" },
            { "I am feeling awesome", "Positive"}
        };

        [Test]
        public async Task GetClassificationResults()
        {
            var api = new OpenAIClient();

            Assert.IsNotNull(api.ClassificationEndpoint);

            var result = await api.ClassificationEndpoint.GetClassificationAsync(new ClassificationRequest(query, examples, labels));
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Label == "Negative");
        }
    }
}