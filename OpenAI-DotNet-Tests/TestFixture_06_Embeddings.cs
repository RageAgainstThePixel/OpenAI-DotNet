using NUnit.Framework;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_06_Embeddings : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_CreateEmbedding()
        {
            Assert.IsNotNull(OpenAIClient.EmbeddingsEndpoint);
            var embedding = await OpenAIClient.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...");
            Assert.IsNotNull(embedding);
            Assert.IsNotEmpty(embedding.Data);
        }

        [Test]
        public async Task Test_2_CreateEmbeddingsWithMultipleInputs()
        {
            Assert.IsNotNull(OpenAIClient.EmbeddingsEndpoint);
            var embeddings = new[]
            {
                "The food was delicious and the waiter...",
                "The food was terrible and the waiter..."
            };
            var embedding = await OpenAIClient.EmbeddingsEndpoint.CreateEmbeddingAsync(embeddings);
            Assert.IsNotNull(embedding);
            Assert.AreEqual(embedding.Data.Count, 2);
        }
    }
}
