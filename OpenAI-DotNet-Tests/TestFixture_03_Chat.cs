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
    }
}