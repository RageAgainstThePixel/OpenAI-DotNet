using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_01_Models
    {
        [Test]
        public async Task Test_1_GetModels()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            var results = await api.ModelsEndpoint.GetModelsAsync();
            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);
        }

        [Test]
        public async Task Test_2_RetrieveModelDetails()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            var models = await api.ModelsEndpoint.GetModelsAsync();

            Console.WriteLine($"Found {models.Count} models!");

            foreach (var model in models)
            {
                Console.WriteLine($"{model.Id} | Owner: {model.OwnedBy}");

                try
                {
                    var result = await api.ModelsEndpoint.GetModelDetailsAsync(model.Id);
                    Assert.IsNotNull(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"No Model details found for {model.Id}\n{e}");
                }
            }
        }
    }
}
