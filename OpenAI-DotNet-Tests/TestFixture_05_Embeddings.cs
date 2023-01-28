using System.Threading.Tasks;
using NUnit.Framework;

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
    }
}
