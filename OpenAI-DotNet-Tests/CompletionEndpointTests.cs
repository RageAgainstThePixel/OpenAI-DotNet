using NUnit.Framework;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI_Tests
{
    public class CompletionEndpointTests
    {
        private readonly string prompts = "One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight";

        [Test]
        public async Task GetBasicCompletion()
        {
            var api = new OpenAIClient();
            Assert.IsNotNull(api.CompletionEndpoint);

            var result = await api.CompletionEndpoint.CreateCompletionAsync(prompts, temperature: 0.1, max_tokens: 5, numOutputs: 5, engine: Engine.Davinci);
            Assert.IsNotNull(result);
            Assert.NotNull(result.Completions);
            Assert.NotZero(result.Completions.Count);
            Assert.That(result.Completions.Any(c => c.Text.Trim().ToLower().StartsWith("nine")));
            Console.WriteLine(result);
        }

        [Test]
        public async Task GetStreamingCompletionAsync()
        {
            var api = new OpenAIClient();
            Assert.IsNotNull(api.CompletionEndpoint);

            var allCompletions = new List<Choice>();

            await api.CompletionEndpoint.StreamCompletionAsync(result =>
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Completions);
                Assert.NotZero(result.Completions.Count);
                allCompletions.AddRange(result.Completions);

                foreach (var choice in result.Completions)
                {
                    Console.WriteLine(choice);
                }
            }, prompts, temperature: 0.1, max_tokens: 5, numOutputs: 5, engine: Engine.Davinci);
            Assert.That(allCompletions.Any(c => c.Text.Trim().ToLower().StartsWith("nine")));
        }


        [Test]
        public async Task GetStreamingEnumerableCompletion()
        {
            var api = new OpenAIClient();
            Assert.IsNotNull(api.CompletionEndpoint);

            var allCompletions = new List<Choice>();

            await foreach (var result in api.CompletionEndpoint.StreamCompletionEnumerableAsync(prompts, temperature: 0.1, max_tokens: 5, numOutputs: 5, engine: Engine.Davinci))
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Completions);
                Assert.NotZero(result.Completions.Count);
                allCompletions.AddRange(result.Completions);
            }

            Assert.That(allCompletions.Any(c => c.Text.Trim().ToLower().StartsWith("nine")));
        }
    }
}