using NUnit.Framework;
using OpenAI.Completions;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_02_Completions
    {
        private readonly string prompts = "One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight";

        [Test]
        public async Task Test_01_GetBasicCompletion()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.CompletionsEndpoint);

            var result = await api.CompletionsEndpoint.CreateCompletionAsync(prompts, temperature: 0.1, maxTokens: 5, numOutputs: 5, model: Model.Davinci);
            Assert.IsNotNull(result);
            Assert.NotNull(result.Completions);
            Assert.NotZero(result.Completions.Count);
            Assert.That(result.Completions.Any(c => c.Text.Trim().ToLower().StartsWith("nine")));

            foreach (var choice in result.Completions)
            {
                Console.WriteLine(choice);
            }
        }

        [Test]
        public async Task Test_02_GetStreamingCompletion()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.CompletionsEndpoint);

            var allCompletions = new List<Choice>();

            await api.CompletionsEndpoint.StreamCompletionAsync(result =>
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Completions);
                Assert.NotZero(result.Completions.Count);
                allCompletions.AddRange(result.Completions);

                foreach (var choice in result.Completions)
                {
                    Console.WriteLine(choice);
                }
            }, prompts, temperature: 0.1, maxTokens: 5, numOutputs: 5, model: Model.Davinci);
            Assert.That(allCompletions.Any(c => c.Text.Trim().ToLower().StartsWith("nine")));
        }

        [Test]
        public async Task Test_03_GetStreamingEnumerableCompletion()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.CompletionsEndpoint);

            var allCompletions = new List<Choice>();

            await foreach (var result in api.CompletionsEndpoint.StreamCompletionEnumerableAsync(prompts, temperature: 0.1, maxTokens: 5, numOutputs: 5, model: Model.Davinci))
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Completions);
                Assert.NotZero(result.Completions.Count);
                Console.WriteLine(result);
                allCompletions.AddRange(result.Completions);
            }

            Assert.That(allCompletions.Any(c => c.Text.Trim().ToLower().StartsWith("nine")));
        }
    }
}
