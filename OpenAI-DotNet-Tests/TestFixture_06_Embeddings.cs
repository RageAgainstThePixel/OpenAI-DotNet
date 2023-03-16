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
            var result = await OpenAIClient.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...");
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Data);
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
            var result = await OpenAIClient.EmbeddingsEndpoint.CreateEmbeddingAsync(embeddings);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Data.Count, 2);
        }
    }
}
