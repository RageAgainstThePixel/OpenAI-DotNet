using System;
using NUnit.Framework;
using OpenAI;

namespace OpenAI_Tests
{
    public class EngineEndpointTests
    {
        [Test]
        public void GetEngines()
        {
            var api = new OpenAIClient(Engine.Davinci);

            var results = api.EnginesEndpoint.GetEnginesAsync().Result;
            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);
        }

        [Test]
        public void RetrieveEngineDetails()
        {
            var api = new OpenAIClient();
            var engines = api.EnginesEndpoint.GetEnginesAsync().Result;

            Console.WriteLine($"Found {engines.Count} engines");

            foreach (var engine in engines)
            {
                Console.WriteLine($"{engine.EngineName} | Owner: {engine.Owner} | ModelRevision: {engine.ModelRevision} | Ready? {engine.Ready}");

                if (engine.Ready.HasValue && engine.Ready.Value)
                {
                    try
                    {
                        var result = api.EnginesEndpoint.GetEngineDetailsAsync(engine.EngineName).Result;
                        Assert.IsNotNull(result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"No Engine details found for {engine.EngineName}\n{e}");
                    }
                }
            }
        }
    }
}