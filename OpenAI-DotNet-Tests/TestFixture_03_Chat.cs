using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_03_Chat
    {
        [Test]
        public async Task Test_1_GetChatCompletion()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ChatEndpoint);
            var chatPrompts = new List<ChatPrompt>
            {
                new ChatPrompt("system", "You are a helpful assistant."),
                new ChatPrompt("user", "Who won the world series in 2020?"),
                new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
                new ChatPrompt("user", "Where was it played?"),
            };
            var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(result);
            Assert.NotNull(result.Choices);
            Assert.NotZero(result.Choices.Count);
            Console.WriteLine(result.FirstChoice);
        }

        [Test]
        public async Task Test_2_GetChatStreamingCompletion()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ChatEndpoint);
            var chatPrompts = new List<ChatPrompt>
            {
                new ChatPrompt("system", "You are a helpful assistant."),
                new ChatPrompt("user", "Who won the world series in 2020?"),
                new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
                new ChatPrompt("user", "Where was it played?"),
            };
            var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);
            var allContent = new List<string>();

            await api.ChatEndpoint.StreamCompletionAsync(chatRequest, result =>
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Choices);
                Assert.NotZero(result.Choices.Count);
                allContent.Add(result.FirstChoice);
            });

            Console.WriteLine(string.Join("", allContent));
        }

        [Test]
        public async Task Test_3_GetChatStreamingCompletionEnumerableAsync()
        {
            var api = new OpenAIClient(OpenAIAuthentication.LoadFromEnv());
            Assert.IsNotNull(api.ChatEndpoint);
            var chatPrompts = new List<ChatPrompt>
            {
                new ChatPrompt("system", "You are a helpful assistant."),
                new ChatPrompt("user", "Who won the world series in 2020?"),
                new ChatPrompt("assistant", "The Los Angeles Dodgers won the World Series in 2020."),
                new ChatPrompt("user", "Where was it played?"),
            };
            var chatRequest = new ChatRequest(chatPrompts, Model.GPT3_5_Turbo);
            var allContent = new List<string>();

            await foreach (var result in api.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
            {
                Assert.IsNotNull(result);
                Assert.NotNull(result.Choices);
                Assert.NotZero(result.Choices.Count);
                allContent.Add(result.FirstChoice);
            }

            Console.WriteLine(string.Join("", allContent));
        }
    }
}