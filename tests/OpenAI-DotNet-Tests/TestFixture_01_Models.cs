using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal sealed class TestFixture_01_Models : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_GetModelsAsync()
        {
            var results = await this.OpenAIClient.ModelsEndpoint.GetModelsAsync();
            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);
        }

        [Test]
        public async Task Test_2_RetrieveModelDetailsAsync()
        {
            var models = await this.OpenAIClient.ModelsEndpoint.GetModelsAsync();
            Console.WriteLine($"Found {models.Count} models!");

            foreach (var model in models.OrderBy(model => model.Id))
            {
                Console.WriteLine($"{model.Id} | Owner: {model.OwnedBy}");

                try
                {
                    var result = await this.OpenAIClient.ModelsEndpoint.GetModelDetailsAsync(model.Id);
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
