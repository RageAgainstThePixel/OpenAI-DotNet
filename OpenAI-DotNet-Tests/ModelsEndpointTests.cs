using NUnit.Framework;
using System;

namespace OpenAI.Tests
{
    public class ModelsEndpointTests
    {
        [Test]
        public void GetModels()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            var results = api.ModelsEndpoint.GetModelsAsync().Result;
            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);
        }

        [Test]
        public void RetrieveModelDetails()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            var models = api.ModelsEndpoint.GetModelsAsync().Result;

            Console.WriteLine($"Found {models.Count} models!");

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
                    Console.WriteLine($"No Model details found for {model.Id}\n{e}");
                }
            }
        }
    }
}
