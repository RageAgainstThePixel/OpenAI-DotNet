using NUnit.Framework;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_06_Embeddings
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

    internal class TestFixture_07_Audio
    {
        [Test]
        public async Task Test_1_Transcription()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.AudioEndpoint);
            await Task.CompletedTask;
        }

        [Test]
        public async Task Test_2_Translation()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.AudioEndpoint);
            await Task.CompletedTask;
        }
    }
}
