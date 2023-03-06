using NUnit.Framework;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_05_Embeddings
    {
        [Test]
        public async Task Test_1_CreateEmbedding()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.EmbeddingsEndpoint);
            var result = await api.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...");
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Data);
        }

        [Test]
        public async Task Test_2_CreateEmbeddingsWithMultipleInputs()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.EmbeddingsEndpoint);
            var embeddings = new[]
            {
                "The food was delicious and the waiter...",
                "The food was terrible and the waiter..."
            };
            var result = await api.EmbeddingsEndpoint.CreateEmbeddingAsync(embeddings);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Data.Count, 2);
        }
    }
}
