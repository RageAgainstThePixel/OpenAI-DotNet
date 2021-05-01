using NUnit.Framework;
using OpenAI_DotNet;
using System;

namespace OpenAI_Tests
{
    public class EngineEndpointTests
    {
        [SetUp]
        public void Setup()
        {
            Authentication.Default = new Authentication(Environment.GetEnvironmentVariable("TEST_OPENAI_SECRET_KEY"));
        }

        [Test]
        public void GetEngines()
        {
            var api = new OpenAI(Engine.Davinci);

            var results = api.EnginesEndpoint.GetEnginesAsync().Result;
            Assert.IsNotNull(results);
            Assert.NotZero(results.Count);
        }

        [Test]
        public void RetrieveEngineDetails()
        {
            var api = new OpenAI(Engine.Davinci);

            var engines = api.EnginesEndpoint.GetEnginesAsync().Result;

            foreach (var engine in engines)
            {
                var result = api.EnginesEndpoint.RetrieveEngineDetailsAsync(engine.EngineName).Result;

                Assert.IsNotNull(result);
            }
        }
    }
}