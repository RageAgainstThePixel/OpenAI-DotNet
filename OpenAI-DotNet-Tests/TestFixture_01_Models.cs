using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_01_Models : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_GetModels()
        {
            var results = await OpenAIClient.ModelsEndpoint.GetModelsAsync();
            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);
        }

        [Test]
        public async Task Test_2_RetrieveModelDetails()
        {
            var models = await OpenAIClient.ModelsEndpoint.GetModelsAsync();
            Console.WriteLine($"Found {models.Count} models!");

            foreach (var model in models.OrderBy(model => model.Id))
            {
                Console.WriteLine($"{model.Id} | Owner: {model.OwnedBy}");

                try
                {
                    var result = await OpenAIClient.ModelsEndpoint.GetModelDetailsAsync(model.Id);
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
