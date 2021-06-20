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
            var api = new OpenAIClient(Engine.Davinci);

            var engines = api.EnginesEndpoint.GetEnginesAsync().Result;

            foreach (var engine in engines)
            {
                var result = api.EnginesEndpoint.GetEngineDetailsAsync(engine.EngineName).Result;

                Assert.IsNotNull(result);
            }
        }
    }
}