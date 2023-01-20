using NUnit.Framework;

namespace OpenAI.Tests
{
    internal class TestFixture_05_Embeddings
    {
        [Test]
        public void Test_1_CreateEmbedding()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.EmbeddingsEndpoint);
            var result = api.EmbeddingsEndpoint.CreateEmbeddingAsync("The food was delicious and the waiter...").Result;
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Data);
        }
    }
}
