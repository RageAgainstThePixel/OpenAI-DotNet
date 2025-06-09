// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Chat;
using OpenAI.Extensions;
using OpenAI.Models;
using OpenAI.Tests.StructuredOutput;
using OpenAI.Tests.Weather;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_04_Chat : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_01_GetChatCompletion()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, "Who won the world series in 2020?"),
                new(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new(Role.User, "Where was it played?")
            };

            var chatRequest = new ChatRequest(messages, Model.GPT4_1_Nano);
            var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsNotEmpty(response.Choices);
            Assert.AreEqual(1, response.Choices.Count);
            Assert.IsNotNull(response.FirstChoice);
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_01_02_GetChatStreamingCompletion()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, "Who won the world series in 2020?"),
                new(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new(Role.User, "Where was it played?")
            };

            var chatRequest = new ChatRequest(messages);
            var cumulativeDelta = string.Empty;

            var response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);

                if (partialResponse.Usage != null)
                {
                    return;
                }

                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);

                foreach (var choice in partialResponse.Choices.Where(choice => choice.Delta?.Content != null))
                {
                    cumulativeDelta += choice.Delta.Content;
                }
            }, true);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            var choice = response.FirstChoice;
            Assert.IsNotNull(choice);
            Assert.IsNotNull(choice.Message);
            Assert.IsFalse(string.IsNullOrEmpty(choice.ToString()));
            Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice} | Finish Reason: {choice.FinishReason}");
            Assert.IsTrue(choice.Message.Role == Role.Assistant);
            Assert.IsTrue(choice.Message.Content!.Equals(cumulativeDelta));
            Console.WriteLine(response.ToString());
            response.PrintUsage();
        }

        [Test]
        public async Task Test_01_03_01_GetChatCompletion_Modalities()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, "Is a golden retriever a good family dog?"),
            };

            var chatRequest = new ChatRequest(messages, Model.GPT4oAudio, audioConfig: Voice.Alloy);
            Assert.IsNotNull(chatRequest);
            Assert.IsNotNull(chatRequest.AudioConfig);
            Assert.AreEqual(Model.GPT4oAudio.Id, chatRequest.Model);
            Assert.AreEqual(Voice.Alloy.Id, chatRequest.AudioConfig.Voice);
            Assert.AreEqual(AudioFormat.Pcm16, chatRequest.AudioConfig.Format);
            Assert.AreEqual(Modality.Text | Modality.Audio, chatRequest.Modalities);
            var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsNotEmpty(response.Choices);
            Assert.AreEqual(1, response.Choices.Count);
            Assert.IsNotNull(response.FirstChoice);
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");
            Assert.IsNotNull(response.FirstChoice.Message.AudioOutput.Data);
            Assert.IsFalse(response.FirstChoice.Message.AudioOutput.Data.IsEmpty);
            response.PrintUsage();

            messages.Add(response.FirstChoice.Message);
            messages.Add(new(Role.User, "What are some other good family dog breeds?"));

            chatRequest = new ChatRequest(messages, Model.GPT4oAudio, audioConfig: Voice.Alloy);
            Assert.IsNotNull(chatRequest);
            Assert.IsNotNull(messages[2]);
            Assert.AreEqual(Role.Assistant, messages[2].Role);
            Assert.IsNotNull(messages[2].AudioOutput);
            response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsNotEmpty(response.Choices);
            Assert.AreEqual(1, response.Choices.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.FirstChoice));
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");
            Assert.IsNotNull(response.FirstChoice.Message.AudioOutput.Data);
            Assert.IsFalse(response.FirstChoice.Message.AudioOutput.Data.IsEmpty);
            response.PrintUsage();
        }

        [Test]
        public async Task Test_01_03_01_GetChatCompletion_Modalities_Streaming()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, "Is a golden retriever a good family dog?"),
            };

            var chatRequest = new ChatRequest(messages, Model.GPT4oAudio, audioConfig: Voice.Alloy);
            Assert.IsNotNull(chatRequest);
            Assert.IsNotNull(chatRequest.AudioConfig);
            Assert.AreEqual(Model.GPT4oAudio.Id, chatRequest.Model);
            Assert.AreEqual(Voice.Alloy.Id, chatRequest.AudioConfig.Voice);
            Assert.AreEqual(AudioFormat.Pcm16, chatRequest.AudioConfig.Format);
            Assert.AreEqual(Modality.Text | Modality.Audio, chatRequest.Modalities);
            var response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, Assert.IsNotNull, true);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsNotEmpty(response.Choices);
            Assert.AreEqual(1, response.Choices.Count);
            Assert.IsNotNull(response.FirstChoice);
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");
            Assert.IsNotEmpty(response.FirstChoice.Message.AudioOutput.Transcript);
            Assert.IsNotNull(response.FirstChoice.Message.AudioOutput.Data);
            Assert.IsFalse(response.FirstChoice.Message.AudioOutput.Data.IsEmpty);
            response.PrintUsage();
            messages.Add(response.FirstChoice.Message);
            messages.Add(new(Role.User, "What are some other good family dog breeds?"));
            chatRequest = new ChatRequest(messages, Model.GPT4oAudio, audioConfig: Voice.Alloy);
            Assert.IsNotNull(chatRequest);
            Assert.IsNotNull(messages[2]);
            Assert.AreEqual(Role.Assistant, messages[2].Role);
            Assert.IsNotNull(messages[2].AudioOutput);
            response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, Assert.IsNotNull, true);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsNotEmpty(response.Choices);
            Assert.AreEqual(1, response.Choices.Count);
            Assert.IsNotEmpty(response.FirstChoice.Message.AudioOutput.Transcript);
            Assert.IsNotNull(response.FirstChoice.Message.AudioOutput.Data);
            Assert.IsFalse(response.FirstChoice.Message.AudioOutput.Data.IsEmpty);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.FirstChoice));
        }

        [Test]
        public async Task Test_01_04_JsonMode()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant designed to output JSON."),
                new(Role.User, "Who won the world series in 2020?"),
            };
#pragma warning disable CS0618 // Type or member is obsolete
            var chatRequest = new ChatRequest(messages, Model.GPT4o, responseFormat: TextResponseFormat.Json);
#pragma warning restore CS0618 // Type or member is obsolete
            var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsNotEmpty(response.Choices);

            foreach (var choice in response.Choices)
            {
                Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice} | Finish Reason: {choice.FinishReason}");
            }

            response.PrintUsage();
        }

        [Test]
        public async Task Test_01_05_01_GetChatStreamingCompletionEnumerableAsync()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, "Who won the world series in 2020?"),
                new(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new(Role.User, "Where was it played?"),
            };

            var cumulativeDelta = string.Empty;
            var chatRequest = new ChatRequest(messages);
            var didThrowException = false;

            await foreach (var partialResponse in OpenAIClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest, true))
            {
                try
                {
                    Assert.IsNotNull(partialResponse);

                    if (partialResponse.Usage != null)
                    {
                        continue;
                    }

                    Assert.NotNull(partialResponse.Choices);
                    Assert.NotZero(partialResponse.Choices.Count);

                    if (partialResponse.FirstChoice?.Delta?.Content is not null)
                    {
                        cumulativeDelta += partialResponse.FirstChoice.Delta.Content;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    didThrowException = true;
                }
            }

            Assert.IsFalse(didThrowException);
            Assert.IsNotEmpty(cumulativeDelta);
            Console.WriteLine(cumulativeDelta);
        }

        [Test]
        public async Task Test_01_05_02_GetChatStreamingModalitiesEnumerableAsync()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, "Count from 1 to 10. Whisper please.")
            };

            var cumulativeDelta = string.Empty;
            using var audioStream = new MemoryStream();
            var chatRequest = new ChatRequest(messages, audioConfig: new AudioConfig(Voice.Nova), model: Model.GPT4oAudio);
            Assert.IsNotNull(chatRequest);
            Assert.IsNotNull(chatRequest.AudioConfig);
            Assert.AreEqual(Model.GPT4oAudio.Id, chatRequest.Model);
            Assert.AreEqual(Voice.Nova.Id, chatRequest.AudioConfig.Voice);
            Assert.AreEqual(AudioFormat.Pcm16, chatRequest.AudioConfig.Format);
            Assert.AreEqual(Modality.Text | Modality.Audio, chatRequest.Modalities);
            var didThrowException = false;

            await foreach (var partialResponse in OpenAIClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest, true))
            {
                try
                {
                    Assert.IsNotNull(partialResponse);

                    if (partialResponse.Usage != null || partialResponse.Choices == null)
                    {
                        continue;
                    }

                    if (partialResponse.FirstChoice?.Delta?.AudioOutput is not null)
                    {
                        await audioStream.WriteAsync(partialResponse.FirstChoice.Delta.AudioOutput.Data);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    didThrowException = true;
                }
            }

            Assert.IsFalse(didThrowException);
            Assert.IsTrue(audioStream.Length > 0);
            Console.WriteLine(cumulativeDelta);
        }

        [Test]
        public async Task Test_02_01_GetChatToolCompletion()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful weather assistant. Always ask the user for their location."),
                new(Role.User, "What's the weather like today?"),
            };

            foreach (var message in messages)
            {
                Console.WriteLine($"{message.Role}: {message.Content}");
            }

            var tools = new List<Tool>
            {
                Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync))
            };

            Assert.IsTrue(tools.All(tool => tool.Function?.Arguments == null));
            var chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "none");
            var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsTrue(response.Choices.Count == 1);
            Assert.IsTrue(response.FirstChoice.FinishReason == "stop");
            messages.Add(response.FirstChoice.Message);

            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");

            var locationMessage = new Message(Role.User, "I'm in Glasgow, Scotland");
            messages.Add(locationMessage);
            Console.WriteLine($"{locationMessage.Role}: {locationMessage.Content}");
            chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "auto");
            response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsTrue(response.Choices.Count == 1);
            messages.Add(response.FirstChoice.Message);

            if (response.FirstChoice.FinishReason == "stop")
            {
                Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");

                var unitMessage = new Message(Role.User, "Fahrenheit");
                messages.Add(unitMessage);
                Console.WriteLine($"{unitMessage.Role}: {unitMessage.Content}");
                chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "auto");
                response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Choices);
                Assert.IsTrue(response.Choices.Count == 1);
            }

            Assert.IsTrue(response.FirstChoice.FinishReason == "tool_calls");
            Assert.IsTrue(response.FirstChoice.Message.ToolCalls.Count == 1);
            var usedTool = response.FirstChoice.Message.ToolCalls[0];
            Assert.IsNotNull(usedTool);
            Assert.IsTrue(usedTool.Function.Name.Contains(nameof(WeatherService.GetCurrentWeatherAsync)));
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {usedTool.Function.Name} | Finish Reason: {response.FirstChoice.FinishReason}");
            Console.WriteLine($"{usedTool.Function.Arguments}");
            var functionResult = await usedTool.InvokeFunctionAsync();
            Assert.IsNotNull(functionResult);
            messages.Add(new Message(usedTool, functionResult));
            Console.WriteLine($"{Role.Tool}: {functionResult}");
            chatRequest = new ChatRequest(messages);
            response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Console.WriteLine(response);
        }

        [Test]
        public async Task Test_02_02_GetChatToolCompletion_Streaming()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful weather assistant. Always prompt the user for their location."),
                new(Role.User, "What's the weather like today?"),
            };

            foreach (var message in messages)
            {
                Console.WriteLine($"{message.Role}: {message.Content}");
            }

            var tools = new List<Tool>
            {
                Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync))
            };

            Assert.IsTrue(tools.All(tool => tool.Function?.Arguments == null));
            var chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "none");

            var response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);

                if (partialResponse.Usage != null)
                {
                    return;
                }

                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);
            }, true);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsTrue(response.Choices.Count == 1);
            messages.Add(response.FirstChoice.Message);

            var locationMessage = new Message(Role.User, "I'm in Glasgow, Scotland");
            messages.Add(locationMessage);
            Console.WriteLine($"{locationMessage.Role}: {locationMessage.Content}");
            chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "auto");

            response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);

                if (partialResponse.Usage != null)
                {
                    return;
                }

                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);
            }, true);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsTrue(response.Choices.Count == 1);
            messages.Add(response.FirstChoice.Message);

            if (response.FirstChoice.FinishReason == "stop")
            {
                Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");

                var unitMessage = new Message(Role.User, "Fahrenheit");
                messages.Add(unitMessage);
                Console.WriteLine($"{unitMessage.Role}: {unitMessage.Content}");
                chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "auto");

                response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
                {
                    Assert.IsNotNull(partialResponse);

                    if (partialResponse.Usage != null)
                    {
                        return;
                    }

                    Assert.NotNull(partialResponse.Choices);
                    Assert.NotZero(partialResponse.Choices.Count);
                }, true);

                Assert.IsNotNull(response);
                Assert.IsNotNull(response.Choices);
                Assert.IsTrue(response.Choices.Count == 1);
            }

            Assert.IsTrue(response.FirstChoice.FinishReason == "tool_calls");
            Assert.IsTrue(response.FirstChoice.Message.ToolCalls.Count == 1);
            var usedTool = response.FirstChoice.Message.ToolCalls[0];
            Assert.IsNotNull(usedTool);
            Assert.IsTrue(usedTool.Function.Name.Contains(nameof(WeatherService.GetCurrentWeatherAsync)));
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {usedTool.Function.Name} | Finish Reason: {response.FirstChoice.FinishReason}");
            Console.WriteLine($"{usedTool.Function.Arguments}");
            var functionResult = await usedTool.InvokeFunctionAsync();
            Assert.IsNotNull(functionResult);
            messages.Add(new Message(usedTool, functionResult));
            Console.WriteLine($"{Role.Tool}: {functionResult}");

            chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "none");

            response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);

                if (partialResponse.Usage != null)
                {
                    return;
                }

                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);
            }, true);

            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_02_03_ChatCompletion_Multiple_Tools_Streaming()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful weather assistant. Use the appropriate unit based on geographical location."),
                new(Role.User, "What's the weather like today in Los Angeles, USA and Tokyo, Japan?"),
            };

            var tools = Tool.GetAllAvailableTools(false, forceUpdate: true, clearCache: true);
            Assert.IsTrue(tools.All(tool => tool.Function?.Arguments == null));
            var chatRequest = new ChatRequest(messages, model: Model.GPT4o, tools: tools, toolChoice: "auto", parallelToolCalls: true);

            var response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);

                if (partialResponse.Usage != null)
                {
                    return;
                }

                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);
            }, true);

            Assert.IsTrue(response.FirstChoice.FinishReason == "tool_calls");
            messages.Add(response.FirstChoice.Message);

            var toolCalls = response.FirstChoice.Message.ToolCalls;

            Assert.NotNull(toolCalls);
            Assert.AreEqual(2, toolCalls.Count);

            foreach (var toolCall in toolCalls)
            {
                var output = await toolCall.InvokeFunctionAsync();
                messages.Add(new Message(toolCall, output));
            }

            chatRequest = new ChatRequest(messages, model: Model.GPT4o, tools: tools, toolChoice: "none");
            response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);

            Assert.IsNotNull(response);
        }

        [Test]
        public async Task Test_02_04_GetChatToolForceCompletion()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful weather assistant. Use the appropriate unit based on geographical location."),
                new(Role.User, "What's the weather like today?"),
            };

            foreach (var message in messages)
            {
                Console.WriteLine($"{message.Role}: {message.Content}");
            }

            var tools = Tool.GetAllAvailableTools(false, forceUpdate: true, clearCache: true);
            Assert.IsTrue(tools.All(tool => tool.Function?.Arguments == null));
            var chatRequest = new ChatRequest(messages, tools: tools, toolChoice: "none");
            var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsTrue(response.Choices.Count == 1);
            Assert.IsTrue(response.FirstChoice.FinishReason == "stop");
            messages.Add(response.FirstChoice.Message);

            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");

            var locationMessage = new Message(Role.User, "I'm in New York, USA");
            messages.Add(locationMessage);
            Console.WriteLine($"{locationMessage.Role}: {locationMessage.Content}");

            chatRequest = new ChatRequest(
                messages,
                tools: tools,
                toolChoice: nameof(WeatherService.GetCurrentWeatherAsync));

            response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsTrue(response.Choices.Count == 1);
            messages.Add(response.FirstChoice.Message);

            Assert.IsTrue(response.FirstChoice.FinishReason == "stop");
            Assert.IsTrue(response.FirstChoice.Message.ToolCalls.Count == 1);
            var usedTool = response.FirstChoice.Message.ToolCalls[0];
            Assert.IsNotNull(usedTool);
            Assert.IsTrue(usedTool.Function.Name.Contains(nameof(WeatherService.GetCurrentWeatherAsync)));
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {usedTool.Function.Name} | Finish Reason: {response.FirstChoice.FinishReason}");
            Console.WriteLine($"{usedTool.Function.Arguments}");
            var functionResult = await usedTool.InvokeFunctionAsync();
            Assert.IsNotNull(functionResult);
            messages.Add(new Message(usedTool, functionResult));
            Console.WriteLine($"{Role.Tool}: {functionResult}");
        }

        [Test]
        public async Task Test_02_05_GetChat_Enumerable_TestToolCalls_Streaming()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You must extract the name from the input"),
                new(Role.User, "My name is Joe")
            };

            var tools = new List<Tool>
            {
                Tool.FromFunc("extract_first_name", (string name) => name)
            };

            var request = new ChatRequest(messages, tools);

            await foreach (var streamResponse in OpenAIClient.ChatEndpoint.StreamCompletionEnumerableAsync(request))
            {
                Console.WriteLine(streamResponse.ToJsonString());

                if (streamResponse.FirstChoice.Message is { } message)
                {
                    foreach (var tool in message.ToolCalls)
                    {
                        var output = tool.InvokeFunction<string>();
                        Console.WriteLine($"Output from StreamCompletionEnumerableAsync: {output}");
                    }
                }
            }
        }

        [Test]
        public async Task Test_03_01_GetChatVision()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, new List<Content>
                {
                    "What's in this image?",
                    new ImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Gfp-wisconsin-madison-the-nature-boardwalk.jpg/2560px-Gfp-wisconsin-madison-the-nature-boardwalk.jpg", ImageDetail.Low)
                })
            };

            var chatRequest = new ChatRequest(messages, model: Model.GPT4o);
            var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishDetails}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_03_02_GetChatVisionStreaming()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, new List<Content>
                {
                    "What's in this image?",
                    new ImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/d/dd/Gfp-wisconsin-madison-the-nature-boardwalk.jpg/2560px-Gfp-wisconsin-madison-the-nature-boardwalk.jpg", ImageDetail.Low)
                })
            };

            var chatRequest = new ChatRequest(messages, model: Model.GPT4o);

            var response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);

                if (partialResponse.Usage != null)
                {
                    return;
                }

                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);
            }, true);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishDetails}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_04_01_GetChatLogProbs()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, "Who won the world series in 2020?"),
                new(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new(Role.User, "Where was it played?"),
            };

            var chatRequest = new ChatRequest(messages, topLogProbs: 1);
            var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsNotEmpty(response.Choices);

            foreach (var choice in response.Choices)
            {
                Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice} | Finish Reason: {choice.FinishReason}");
            }

            response.PrintUsage();
        }

        [Test]
        public async Task Test_04_02_GetChatLogProbsStreaming()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful assistant."),
                new(Role.User, "Who won the world series in 2020?"),
                new(Role.Assistant, "The Los Angeles Dodgers won the World Series in 2020."),
                new(Role.User, "Where was it played?"),
            };

            var chatRequest = new ChatRequest(messages, topLogProbs: 1);
            var cumulativeDelta = string.Empty;

            var response = await OpenAIClient.ChatEndpoint.StreamCompletionAsync(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);

                if (partialResponse.Usage != null)
                {
                    return;
                }

                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);

                foreach (var choice in partialResponse.Choices.Where(choice => choice.Delta?.Content != null))
                {
                    cumulativeDelta += choice.Delta.Content;
                }
            }, true);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            var choice = response.FirstChoice;
            Assert.IsNotNull(choice);
            Assert.IsNotNull(choice.Message);
            Assert.IsFalse(string.IsNullOrEmpty(choice.ToString()));
            Console.WriteLine($"[{choice.Index}] {choice.Message.Role}: {choice} | Finish Reason: {choice.FinishReason}");
            Assert.IsTrue(choice.Message.Role == Role.Assistant);
            Assert.IsTrue(choice.Message.Content!.Equals(cumulativeDelta));
            Console.WriteLine(response.ToString());
            response.PrintUsage();
        }

        [Test]
        public async Task Test_05_01_GetChat_JsonSchema()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful math tutor. Guide the user through the solution step by step."),
                new(Role.User, "how can I solve 8x + 7 = -23")
            };

            var chatRequest = new ChatRequest(messages, model: "gpt-4o-2024-08-06");
            var (mathResponse, chatResponse) = await OpenAIClient.ChatEndpoint.GetCompletionAsync<MathResponse>(chatRequest);
            Assert.IsNotNull(chatResponse);
            Assert.IsNotNull(mathResponse);
            Assert.IsNotEmpty(mathResponse.Steps);
            Assert.IsNotNull(chatResponse.Choices);
            Assert.IsNotEmpty(chatResponse.Choices);

            for (var i = 0; i < mathResponse.Steps.Count; i++)
            {
                var step = mathResponse.Steps[i];
                Assert.IsNotNull(step.Explanation);
                Console.WriteLine($"Step {i}: {step.Explanation}");
                Assert.IsNotNull(step.Output);
                Console.WriteLine($"Result: {step.Output}");
            }

            Assert.IsNotNull(mathResponse.FinalAnswer);
            Console.WriteLine($"Final Answer: {mathResponse.FinalAnswer}");

            chatResponse.PrintUsage();
        }

        [Test]
        public async Task Test_05_02_GetChat_JsonSchema_Streaming()
        {
            Assert.IsNotNull(OpenAIClient.ChatEndpoint);

            var messages = new List<Message>
            {
                new(Role.System, "You are a helpful math tutor. Guide the user through the solution step by step."),
                new(Role.User, "how can I solve 8x + 7 = -23")
            };

            var chatRequest = new ChatRequest(messages, model: "gpt-4o-2024-08-06");
            var cumulativeDelta = string.Empty;

            var (mathResponse, chatResponse) = await OpenAIClient.ChatEndpoint.StreamCompletionAsync<MathResponse>(chatRequest, partialResponse =>
            {
                Assert.IsNotNull(partialResponse);

                if (partialResponse.Usage != null)
                {
                    return;
                }

                Assert.NotNull(partialResponse.Choices);
                Assert.NotZero(partialResponse.Choices.Count);

                foreach (var choice in partialResponse.Choices.Where(choice => choice.Delta?.Content != null))
                {
                    cumulativeDelta += choice.Delta.Content;
                }
            }, true);

            Assert.IsNotNull(chatResponse);
            Assert.IsNotNull(mathResponse);
            Assert.IsNotNull(chatResponse.Choices);
            var choice = chatResponse.FirstChoice;
            Assert.IsNotNull(choice);
            Assert.IsNotNull(choice.Message);
            Assert.IsFalse(string.IsNullOrEmpty(choice.ToString()));
            Assert.IsTrue(choice.Message.Role == Role.Assistant);
            Assert.IsTrue(choice.Message.Content!.Equals(cumulativeDelta));

            for (var i = 0; i < mathResponse.Steps.Count; i++)
            {
                var step = mathResponse.Steps[i];
                Assert.IsNotNull(step.Explanation);
                Console.WriteLine($"Step {i}: {step.Explanation}");
                Assert.IsNotNull(step.Output);
                Console.WriteLine($"Result: {step.Output}");
            }

            Assert.IsNotNull(mathResponse.FinalAnswer);
            Console.WriteLine($"Final Answer: {mathResponse.FinalAnswer}");

            chatResponse.PrintUsage();
        }

        [Test]
        public async Task Test_06_01_GetReasoningResponse()
        {
            const string POLICY = "policy = \"\n# TechStore Policy: Customer Service Guidelines\n\n## 1. Introduction\n\nWelcome to our Customer Service Guidelines for TechStore. This policy aims to provide comprehensive guidance to resolve common customer concerns, covering multiple scenarios, including product damage, incorrect orders, refunds, and replacements. The guidelines include decision trees to help agents make informed decisions and offer a consistent customer experience.\n\n## 2. Product Damage Policy\n\nOur product damage policy covers a range of potential issues, from delivery damage to manufacturing defects. The following decision tree helps guide customer service representatives through different scenarios related to damaged products.\n\n### Decision Tree: Damaged Product Resolution\n\n- **Step 1: Damage Reported**\n\n  - **If there is visible damage** (e.g., cracked screen, broken case):\n    Proceed to **Step 2**.\n\n  - **If there is no visible damage**:\n    Proceed to **Step 3c**.\n\n- **Step 2:** Was the damage reported within 7 days of delivery?\n\n  - **Yes:** Proceed to **Step 3a**.\n  - **No:** Proceed to **Step 3b**.\n\n- **Step 3: Eligibility for Replacement/Refund**\n\n  - **a. Visible Damage Reported Within 7 Days:**\n    - **Option 1:** Full replacement if stock is available.\n    - **Option 2:** Refund (customer's choice).\n    - **Agent Action:** Confirm customer's preferred resolution and provide prepaid shipping label for return (if needed).\n  - **b. Visible Damage Reported After 7 Days but Within 30 Days:**\n    - Assess if the damage could be due to customer mishandling.\n      - **Not Customer Fault (e.g. internal screen issue, phone burst):** Offer repair service or replacement (partial fees may apply based on damage).\n      - **Customer Mishandling (e.g. cracked screen, visible damage):** Offer paid repair or replacement, but customer is not eligible for a refund\n  - **c. No visible damage: Assess damage type**\n    - **If battery issue**: Offer discount for new phone or paid replacement of battery\n    - **If software issue**: Offer troubleshooting tips (see troubleshooting section)\n    - **If phone turns off**: Proceed to **Step 4**.\n\n- **Step 4:** Check Product Purchase Date:\n  - **Less than 1 Year:** Eligible for replacement or return with similar product up to 120% value.\n  - **Less than 1 Month:** Eligible for replacement or return with similar product up to 130% value.\n  - **Between 1 and 2 Years:** Eligible for replacement with similar product up to 110% value.\n  - **More than 2 Years:** No refund or replacement possible.\n\n## 3. Incorrect Order Policy\n\n### Decision Tree: Incorrect Order Handling\n\n- **Step 1: Wrong Product Received**\n\n  - **Customer Receives Wrong Item:**\n    - Ask for photo proof of the item received.\n    - Confirm if the product matches anything available in our inventory (to check if it was a mix-up).\n      - **Match Found:** Proceed to **Step 2**.\n      - **No Match Found:** Escalate to the inventory team to verify any logistical errors.\n\n- **Step 2: Replacement and Retrieval**\n\n  - Confirm that the wrong item is in unused condition and eligible for return.\n  - Offer prepaid shipping label for return of incorrect item.\n  - **Stock Availability for Correct Item:**\n    - **In Stock:** Dispatch correct item immediately.\n    - **Out of Stock:** Offer the following options to the customer:\n      - **Option 1:** Full refund.\n      - **Option 2:** Store credit with 10% bonus as goodwill.\n      - **Option 3:** Waitlist for restock.\n\n## 4. Refund Policy\n\nRefunds may be processed depending on specific scenarios and customer preferences. Use the decision tree below to guide customers.\n\n### Decision Tree: Refund Processing\n\n- **Step 1: Reason for Refund Request**\n\n  - **Product Damaged:** Follow guidelines under **Product Damage Policy**.\n  - **Customer Unsatisfied with Product:**\n    - **Step 2:** Is the request made within 30 days of purchase?\n      - **Yes:** Offer refund (excluding return shipping fee) or store credit.\n      - **No:** Advise customer of the 30-day return limit, suggest troubleshooting, or discuss potential product upgrade.\n  - **Wrong Product Delivered:** See **Incorrect Order Policy** for replacement and refund eligibility.\n\n- **Step 3: Condition of Returned Product**\n\n  - **Unused, Original Packaging:** Full refund processed within 5-7 business days.\n  - **Used, but Fault-Free:** Charge restocking fee (15%) and process the remainder of the refund.\n  - **Damaged Due to Customer Handling:** Notify customer of deduction in refund to cover repair/restocking fee.\n\n## 5. Warranty Claims\n\nAll wearable hardware products come with a limited one-year warranty covering manufacturer defects. Warranty claims are subject to the following decision tree.\n\n### **Decision Tree: Warranty Claim Evaluation**\n\n- **Step 1: Warranty Validity**\n\n  - **Product Purchase Date Verified:**\n    - Within 1 year?\n      - **Yes:** Proceed to **Step 2**.\n      - **No:** Offer paid repair options.\n\n- **Step 2: Nature of Defect**\n\n  - **Manufacturing Defect Confirmed:**\n    - Offer free replacement or repair.\n  - **Wear and Tear or Customer Neglect:**\n    - Inform customer that warranty does not cover general wear and tear.\n    - Offer discounted repair.\n\n## 6. Customer Courtesy Compensation\n\nIn certain cases, customers may be eligible for courtesy compensation in the form of store credit or discounts.\n\n### Decision Tree: Eligibility for Courtesy Compensation\n\n- **Step 1: Assess Severity of Issue**\n  - **Major Inconvenience Due to Our Error:** (e.g., repeated delivery issues, incorrect items sent multiple times)\n    - Offer store credit equivalent to 15% of the product value or a discount coupon for future purchases.\n  - **Minor Issue:** (e.g., delayed delivery due to courier but product received in good condition)\n    - Offer free accessory or a 5% discount code.\n\n## 7. Summary of Escalation Paths\n\nFor cases that cannot be resolved using the above decision trees, escalate to:\n\n- **Tier 2 Support:** Issues requiring deeper technical knowledge (e.g., rare hardware malfunctions, software issues that persist after troubleshooting).\n- **Logistics Team:** Situations involving repeated wrong item deliveries, significant delays, or lost packages.\n- **Customer Success Manager:** High-value customers, chronic dissatisfaction, or VIP escalations requiring a tailored solution.\n\n## 8. Record-Keeping and Follow-Ups\n\n- Ensure all interactions are logged in the CRM system.\n- Follow up on any pending replacements, warranty claims, or courtesy compensation within 48 hours.\n- Provide customers with tracking numbers and regular updates on their cases.\n\n## 9. Final Notes\n\nThese guidelines are intended to ensure our customers are supported in every scenario with fair, efficient, and courteous service. Always strive for the best customer experience, listen attentively, and seek the best solution that aligns with both customer needs and company policy.\n";
            const string REQUEST_SUMMARY = "Phone keeps turning off, it has no visible damage";
            const string ORDER_CONTEXT = "Samsung Galaxy S23 bought on January 28th, 2024.";

            Assert.IsNotNull(OpenAIClient.ChatEndpoint);
            var messages = new List<Message>
            {
                new(Role.Developer, $"This is our customer service policy:\n{POLICY}\n\nToday is {DateTime.UtcNow}.\n\nYou will be provided with context on a user order.\nYour task is to reply with what the user is eligible for.\nRespond in one sentence."),
                new(Role.User, $"Here is context on the order:\nRequest Summary: {REQUEST_SUMMARY}\nOrder Context: {ORDER_CONTEXT}")
            };

            var chatRequest = new ChatRequest(messages, model: Model.O3Mini);
            var response = await OpenAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Choices);
            Assert.IsNotEmpty(response.Choices);
            Console.WriteLine($"{response.FirstChoice.Message.Role}: {response.FirstChoice} | Finish Reason: {response.FirstChoice.FinishReason}");
            response.PrintUsage();
        }
    }
}
