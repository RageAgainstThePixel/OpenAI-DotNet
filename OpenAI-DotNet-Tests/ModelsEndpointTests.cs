using NUnit.Framework;
using OpenAI.Models;
using System;

namespace OpenAI.Tests
{
    public class ModelsEndpointTests
    {
        [Test]
        public void GetEngines()
        {
            var api = new OpenAIClient(Model.Davinci);

            var results = api.ModelsEndpoint.GetModelsAsync().Result;
            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);
        }

        [Test]
        public void RetrieveModelDetails()
        {
            var api = new OpenAIClient();
            var models = api.ModelsEndpoint.GetModelsAsync().Result;

            Console.WriteLine($"Found {models.Count} engines!");

            foreach (var model in models)
            {
                Console.WriteLine($"{model.Id} | Owner: {model.OwnedBy}");

                try
                {
                    var result = api.ModelsEndpoint.GetModelDetailsAsync(model.Id).Result;
                    Assert.IsNotNull(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"No Engine details found for {model.Id}\n{e}");
                }
            }
        }
    }
}