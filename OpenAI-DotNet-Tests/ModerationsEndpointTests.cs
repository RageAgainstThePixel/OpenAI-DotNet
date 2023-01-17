using NUnit.Framework;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    public class ModerationsEndpointTests
    {
        [Test]
        public async Task GetModerationBadAsync()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.CompletionsEndpoint);

            var result = await api.ModerationsEndpoint.GetModerationAsync("Nudity sex test");
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
            Console.WriteLine(result);
        }

        [Test]
        public async Task GetModerationGoodAsync()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.CompletionsEndpoint);

            var result = await api.ModerationsEndpoint.GetModerationAsync("Find joy in life's journey");
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
            Console.WriteLine(result);
        }
    }
}