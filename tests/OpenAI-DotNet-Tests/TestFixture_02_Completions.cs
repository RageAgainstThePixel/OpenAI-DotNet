using NUnit.Framework;
using OpenAI.Completions;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal sealed class TestFixture_02_Completions : AbstractTestFixture
    {
        private const string CompletionPrompts = "One Two Three Four Five Six Seven Eight Nine One Two Three Four Five Six Seven Eight";

        [Test]
        public async Task Test_01_GetBasicCompletionAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.CompletionsEndpoint);
            var result = await this.OpenAIClient.CompletionsEndpoint.CreateCompletionAsync(
                CompletionPrompts,
                temperature: 0.1,
                maxTokens: 5,
                numOutputs: 5,
                model: Model.Davinci);
            Assert.IsNotNull(result);
            Assert.NotNull(result.Completions);
            Assert.NotZero(result.Completions.Count);
            Assert.That(result.Completions.Any(c => c.Text.Trim().ToLower(CultureInfo.CurrentCulture).StartsWith("nine", StringComparison.OrdinalIgnoreCase)));
            Console.WriteLine(result);
        }

        [Test]
        public async Task Test_02_GetStreamingCompletionAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.CompletionsEndpoint);
            var allCompletions = new List<Choice>();

            await this.OpenAIClient.CompletionsEndpoint.StreamCompletionAsync(result =>
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Completions);
                Assert.NotZero(result.Completions.Count);
                allCompletions.AddRange(result.Completions);
            }, CompletionPrompts, temperature: 0.1, maxTokens: 5, numOutputs: 5);

            Assert.That(allCompletions.Any(c => c.Text.Trim().ToLower(CultureInfo.CurrentCulture).StartsWith("nine", StringComparison.OrdinalIgnoreCase)));
            Console.WriteLine(allCompletions.FirstOrDefault());
        }

        [Test]
        public async Task Test_03_GetStreamingEnumerableCompletionAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.CompletionsEndpoint);
            var allCompletions = new List<Choice>();

            await foreach (var result in this.OpenAIClient.CompletionsEndpoint.StreamCompletionEnumerableAsync(
                               CompletionPrompts,
                               temperature: 0.1,
                               maxTokens: 5,
                               numOutputs: 5,
                               model: Model.Davinci))
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Completions);
                Assert.NotZero(result.Completions.Count);
                allCompletions.AddRange(result.Completions);
            }

            Assert.That(allCompletions.Any(c => c.Text.Trim().ToLower(CultureInfo.CurrentCulture).StartsWith("nine", StringComparison.OrdinalIgnoreCase)));
            Console.WriteLine(allCompletions.FirstOrDefault());
        }
    }
}
