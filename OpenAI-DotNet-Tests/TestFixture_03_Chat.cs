using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Tests.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_03_Chat : AbstractTestFixture
    {
        [Test]
        public async Task Test_1_GetChatCompletion()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);
            var messages = new List<Message>
            {
                new Message(Role.System, "You are a helpful assistant."),
                new Message(Role.User, "Who won the world series in 2020?"),
                new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new Message(Role.User, "Where was it played?"),
            };
            var chatRequest = new ChatRequest(messages, number: 2);
            var result = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Choices);
            Assert.IsTrue(result.Choices.Count == 2);

            foreach (var choice in result.Choices)
            {
                Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message.Content} | Finish Reason: {choice.FinishReason}");
            }

            result.GetUsage();
        }

        [Test]
        public async Task Test_2_GetChatStreamingCompletion()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);
            const int choiceCount = 2;
            var messages = new List<Message>
            {
                new Message(Role.System, "You are a helpful assistant."),
                new Message(Role.User, "Who won the world series in 2020?"),
                new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new Message(Role.User, "Where was it played?"),
            };
            var chatRequest = new ChatRequest(messages, number: choiceCount);
            var cumulativeDelta = new List<string>();

            for (var i = 0; i < choiceCount; i++)
            {
                cumulativeDelta.Add(string.Empty);
            }

            var response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);
                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);

                foreach (var choice in partialResponse.Choices.Where(choice => choice.Delta?.Content != null))
                {
                    Console.WriteLine($"[{choice.Index}] {choice.Delta.Content}");
                    cumulativeDelta[choice.Index] += choice.Delta.Content;
                }
            });

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsTrue(response.Choices.Count == choiceCount);

            for (var i = 0; i < choiceCount; i++)
            {
                var choice = response.Choices[i];
                Assert.IsFalse(string.IsNullOrEmpty(choice?.Message?.Content));
                Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message.Content} | Finish Reason: {choice.FinishReason}");
                Assert.IsTrue(choice.Message.Role == Role.Assistant);
                var deltaContent = cumulativeDelta[i];
                Assert.IsTrue(choice.Message.Content.Equals(deltaContent));
            }

            response.GetUsage();
        }

        [Test]
        public async Task Test_3_GetChatStreamingCompletionEnumerableAsync()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);
            var messages = new List<Message>
            {
                new Message(Role.System, "You are a helpful assistant."),
                new Message(Role.User, "Who won the world series in 2020?"),
                new Message(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new Message(Role.User, "Where was it played?"),
            };
            var chatRequest = new ChatRequest(messages, number: 2);
            await foreach (var result in OpenAIClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
            {
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Choices);
                Assert.NotZero(result.Choices.Count);

                foreach (var choice in result.Choices.Where(choice => !string.IsNullOrEmpty(choice.Delta?.Content)))
                {
                    Console.WriteLine($"[{choice.Index}] {choice.Delta.Content}");
                }

                foreach (var choice in result.Choices.Where(choice => !string.IsNullOrEmpty(choice.Message?.Content)))
                {
                    Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message.Content} | Finish Reason: {choice.FinishReason}");
                }
            }
        }

        [Test]
        public async Task Test_4_GetChatFunctionCompletion()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);
            var messages = new List<Message>
            {
                new Message(Role.System, "You are a helpful weather assistant."),
                new Message(Role.User, "What's the weather like today?"),
            };

            foreach (var message in messages)
            {
                Console.WriteLine($"{message.Role}: {message.Content}");
            }

            var functions = new List<Function>
            {
                new Function(
                    nameof(WeatherService.GetCurrentWeather),
                    "Get the current weather in a given location",
                     new JsonObject
                     {
                         ["type"] = "object",
                         ["properties"] = new JsonObject
                         {
                             ["location"] = new JsonObject
                             {
                                 ["type"] = "string",
                                 ["description"] = "The city and state, e.g. San Francisco, CA"
                             },
                             ["unit"] = new JsonObject
                             {
                                 ["type"] = "string",
                                 ["enum"] = new JsonArray {"celsius", "fahrenheit"}
                             }
                         },
                         ["required"] = new JsonArray { "location", "unit" }
                     })
            };

            var chatRequest = new ChatRequest(messages, functions: functions, functionCall: "auto", model: "gpt-3.5-turbo");
            var result = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Choices);
            Assert.IsTrue(result.Choices.Count == 1);
            messages.Add(result.FirstChoice.Message);

            Console.WriteLine($"{result.FirstChoice.Message.Role}: {result.FirstChoice.Message.Content} | Finish Reason: {result.FirstChoice.FinishReason}");

            var locationMessage = new Message(Role.User, "I'm in Glasgow, Scotland");
            messages.Add(locationMessage);
            Console.WriteLine($"{locationMessage.Role}: {locationMessage.Content}");
            chatRequest = new ChatRequest(messages, functions: functions, functionCall: "auto", model: "gpt-3.5-turbo");
            result = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Choices);
            Assert.IsTrue(result.Choices.Count == 1);
            messages.Add(result.FirstChoice.Message);

            if (!string.IsNullOrEmpty(result.FirstChoice.Message.Content))
            {
                Console.WriteLine($"{result.FirstChoice.Message.Role}: {result.FirstChoice.Message.Content} | Finish Reason: {result.FirstChoice.FinishReason}");

                var unitMessage = new Message(Role.User, "celsius");
                messages.Add(unitMessage);
                Console.WriteLine($"{unitMessage.Role}: {unitMessage.Content}");
                chatRequest = new ChatRequest(messages, functions: functions, functionCall: "auto", model: "gpt-3.5-turbo");
                result = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Choices);
                Assert.IsTrue(result.Choices.Count == 1);
            }

            Assert.IsTrue(result.FirstChoice.FinishReason == "function_call");
            Assert.IsTrue(result.FirstChoice.Message.Function.Name == nameof(WeatherService.GetCurrentWeather));
            Console.WriteLine($"{result.FirstChoice.Message.Role}: {result.FirstChoice.Message.Function.Name} | Finish Reason: {result.FirstChoice.FinishReason}");
            Console.WriteLine($"{result.FirstChoice.Message.Function.Arguments}");
            var functionArgs = JsonSerializer.Deserialize<WeatherArgs>(result.FirstChoice.Message.Function.Arguments.ToString());
            var functionResult = WeatherService.GetCurrentWeather(functionArgs);
            Assert.IsNotNull(functionResult);
            messages.Add(new Message(Role.Function, functionResult, nameof(WeatherService.GetCurrentWeather)));
            Console.WriteLine($"{Role.Function}: {functionResult}");
            chatRequest = new ChatRequest(messages, functions: functions, functionCall: "auto", model: "gpt-3.5-turbo-0613");
            result = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Console.WriteLine(result);
        }

        [Test]
        public async Task Test_5_GetChatFunctionCompletion_Streaming()
        {
            var messages = new List<Message>
            {
                new Message(Role.System, "You are a helpful weather assistant."),
                new Message(Role.User, "What's the weather like today?"),
            };

            foreach (var message in messages)
            {
                Console.WriteLine($"{message.Role}: {message.Content}");
            }

            var functions = new List<Function>
            {
                new Function(
                    nameof(WeatherService.GetCurrentWeather),
                    "Get the current weather in a given location",
                    new JsonObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JsonObject
                        {
                            ["location"] = new JsonObject
                            {
                                ["type"] = "string",
                                ["description"] = "The city and state, e.g. San Francisco, CA"
                            },
                            ["unit"] = new JsonObject
                            {
                                ["type"] = "string",
                                ["enum"] = new JsonArray {"celsius", "fahrenheit"}
                            }
                        },
                        ["required"] = new JsonArray { "location", "unit" }
                    })
            };

            var chatRequest = new ChatRequest(messages, functions: functions, functionCall: "auto", model: "gpt-3.5-turbo");
            var result = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);
                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);

                foreach (var choice in partialResponse.Choices.Where(choice => !string.IsNullOrEmpty(choice.Delta?.Content)))
                {
                    Console.WriteLine($"{choice.Delta.Content}");
                }

                foreach (var choice in partialResponse.Choices.Where(choice => !string.IsNullOrEmpty(choice.Message?.Content)))
                {
                    Console.WriteLine($"{choice.Message.Role}: {choice.Message.Content} | Finish Reason: {choice.FinishReason}");
                }
            });
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Choices);
            Assert.IsTrue(result.Choices.Count == 1);
            messages.Add(result.FirstChoice.Message);

            var locationMessage = new Message(Role.User, "I'm in Glasgow, Scotland");
            messages.Add(locationMessage);
            Console.WriteLine($"{locationMessage.Role}: {locationMessage.Content}");
            chatRequest = new ChatRequest(messages, functions: functions, functionCall: "auto", model: "gpt-3.5-turbo");
            result = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);
                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);

                foreach (var choice in partialResponse.Choices.Where(choice => choice.Delta?.Content != null))
                {
                    Console.WriteLine($"[{choice.Index}] {choice.Delta.Content}");
                }

                foreach (var choice in partialResponse.Choices.Where(choice => choice.Message?.Content != null))
                {
                    Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice.Message.Content} | Finish Reason: {choice.FinishReason}");
                }
            });
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Choices);
            Assert.IsTrue(result.Choices.Count == 1);
            messages.Add(result.FirstChoice.Message);

            if (!string.IsNullOrEmpty(result.FirstChoice.Message.Content))
            {
                Console.WriteLine($"{result.FirstChoice.Message.Role}: {result.FirstChoice.Message.Content} | Finish Reason: {result.FirstChoice.FinishReason}");

                var unitMessage = new Message(Role.User, "celsius");
                messages.Add(unitMessage);
                Console.WriteLine($"{unitMessage.Role}: {unitMessage.Content}");
                chatRequest = new ChatRequest(messages, functions: functions, functionCall: "auto", model: "gpt-3.5-turbo");
                result = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
                {
                    Assert.IsNotNull(partialResponse);
                    Assert.NotNull(partialResponse.Choices);
                    Assert.NotZero(partialResponse.Choices.Count);

                    foreach (var choice in partialResponse.Choices.Where(choice => choice.Delta?.Content != null))
                    {
                        Console.WriteLine($"{choice.Delta.Content}");
                    }

                    foreach (var choice in partialResponse.Choices.Where(choice => choice.Message?.Content != null))
                    {
                        Console.WriteLine($"{choice.Message.Role}: {choice.Message.Content} | Finish Reason: {choice.FinishReason}");
                    }
                });
                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Choices);
                Assert.IsTrue(result.Choices.Count == 1);
            }

            Assert.IsTrue(result.FirstChoice.FinishReason == "function_call");
            Assert.IsTrue(result.FirstChoice.Message.Function.Name == nameof(WeatherService.GetCurrentWeather));
            Console.WriteLine($"{result.FirstChoice.Message.Role}: {result.FirstChoice.Message.Function.Name} | Finish Reason: {result.FirstChoice.FinishReason}");
            Console.WriteLine($"{result.FirstChoice.Message.Function.Arguments}");

            var functionArgs = JsonSerializer.Deserialize<WeatherArgs>(result.FirstChoice.Message.Function.Arguments.ToString());
            var functionResult = WeatherService.GetCurrentWeather(functionArgs);
            Assert.IsNotNull(functionResult);
            messages.Add(new Message(Role.Function, functionResult, nameof(WeatherService.GetCurrentWeather)));
            Console.WriteLine($"{Role.Function}: {functionResult}");
        }

        [Test]
        public async Task Test_6_GetChatFunctionForceCompletion()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);
            var messages = new List<Message>
            {
                new Message(Role.System, "You are a helpful weather assistant."),
                new Message(Role.User, "What's the weather like today?"),
            };

            foreach (var message in messages)
            {
                Console.WriteLine($"{message.Role}: {message.Content}");
            }

            var functions = new List<Function>
            {
                new Function(
                    nameof(WeatherService.GetCurrentWeather),
                    "Get the current weather in a given location",
                     new JsonObject
                     {
                         ["type"] = "object",
                         ["properties"] = new JsonObject
                         {
                             ["location"] = new JsonObject
                             {
                                 ["type"] = "string",
                                 ["description"] = "The city and state, e.g. San Francisco, CA"
                             },
                             ["unit"] = new JsonObject
                             {
                                 ["type"] = "string",
                                 ["enum"] = new JsonArray {"celsius", "fahrenheit"}
                             }
                         },
                         ["required"] = new JsonArray { "location", "unit" }
                     })
            };

            var chatRequest = new ChatRequest(messages, functions: functions, functionCall: null, model: "gpt-3.5-turbo");
            var result = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Choices);
            Assert.IsTrue(result.Choices.Count == 1);
            messages.Add(result.FirstChoice.Message);

            Console.WriteLine($"{result.FirstChoice.Message.Role}: {result.FirstChoice.Message.Content} | Finish Reason: {result.FirstChoice.FinishReason}");

            var locationMessage = new Message(Role.User, "I'm in Glasgow, Scotland");
            messages.Add(locationMessage);
            Console.WriteLine($"{locationMessage.Role}: {locationMessage.Content}");
            chatRequest = new ChatRequest(
                messages,
                functions: functions,
                functionCall: nameof(WeatherService.GetCurrentWeather),
                model: "gpt-3.5-turbo-0613");
            result = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Choices);
            Assert.IsTrue(result.Choices.Count == 1);
            messages.Add(result.FirstChoice.Message);

            Assert.IsTrue(result.FirstChoice.FinishReason == "stop");
            Assert.IsTrue(result.FirstChoice.Message.Function.Name == nameof(WeatherService.GetCurrentWeather));
            Console.WriteLine($"{result.FirstChoice.Message.Role}: {result.FirstChoice.Message.Function.Name} | Finish Reason: {result.FirstChoice.FinishReason}");
            Console.WriteLine($"{result.FirstChoice.Message.Function.Arguments}");
            var functionArgs = JsonSerializer.Deserialize<WeatherArgs>(result.FirstChoice.Message.Function.Arguments.ToString());
            var functionResult = WeatherService.GetCurrentWeather(functionArgs);
            Assert.IsNotNull(functionResult);
            messages.Add(new Message(Role.Function, functionResult, nameof(WeatherService.GetCurrentWeather)));
            Console.WriteLine($"{Role.Function}: {functionResult}");
        }
    }
}