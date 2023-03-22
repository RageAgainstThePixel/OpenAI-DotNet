using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal sealed class TestFixture_03_Chat : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_GetChatCompletionAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.ChatEndpoint);
            var chatPrompts = new List<ChatPrompt>
            {
                new ChatPrompt("system", "You are a helpful assistant."),
                new ChatPrompt("user", "Who won the world series in 2020?"),
                new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
                new ChatPrompt("user", "Where was it played?"),
            };
            var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);
            var result = await this.OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(result);
            Assert.NotNull(result.Choices);
            Assert.NotZero(result.Choices.Count);
            Console.WriteLine(result.FirstChoice);
        }

        [Test]
        public async Task Test_2_GetChatStreamingCompletionAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.ChatEndpoint);
            var chatPrompts = new List<ChatPrompt>
            {
                new ChatPrompt("system", "You are a helpful assistant."),
                new ChatPrompt("user", "Who won the world series in 2020?"),
                new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
                new ChatPrompt("user", "Where was it played?"),
            };
            var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);
            var allContent = new List<string>();

            await this.OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, result =>
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Choices);
                Assert.NotZero(result.Choices.Count);
                allContent.Add(result.FirstChoice);
            });

            Console.WriteLine(String.Join("", allContent));
        }

        [Test]
        public async Task Test_3_GetChatStreamingCompletionEnumerableAsync()
        {
            Assert.IsNotNull(this.OpenAIClient.ChatEndpoint);
            var chatPrompts = new List<ChatPrompt>
            {
                new ChatPrompt("system", "You are a helpful assistant."),
                new ChatPrompt("user", "Who won the world series in 2020?"),
                new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
                new ChatPrompt("user", "Where was it played?"),
            };
            var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);
            var allContent = new List<string>();

            await foreach (var result in this.OpenAIClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Choices);
                Assert.NotZero(result.Choices.Count);
                allContent.Add(result.FirstChoice);
            }

            Console.WriteLine(String.Join("", allContent));
        }
    }
}