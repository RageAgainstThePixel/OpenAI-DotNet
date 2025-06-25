// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Models;
using OpenAI.Responses;
using OpenAI.Tests.StructuredOutput;
using OpenAI.Tests.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_14_Responses : AbstractTestFixture
    {
        [Test]
        public void Test_00_01_ResponseRequest_Serialization()
        {
            const string content = "Tell me a three sentence bedtime story about a unicorn.";
            CreateResponseRequest request = content;
            Assert.NotNull(request);
            Assert.IsNotEmpty(request.Input);
            Assert.IsNotNull(request.Input[0]);
            Assert.IsInstanceOf<Message>(request.Input[0]);
            var messageItem = request.Input[0] as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            var textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Assert.AreEqual(content, textContent.Text);
            var jsonPayload = JsonSerializer.Serialize(request, new JsonSerializerOptions(OpenAIClient.JsonSerializationOptions)
            {
                WriteIndented = true
            });
            Assert.IsNotEmpty(jsonPayload);
            Console.WriteLine(jsonPayload);
        }

        [Test]
        public async Task Test_01_01_SimpleTextInput()
        {
            Assert.NotNull(OpenAIClient.ResponsesEndpoint);
            var response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync("Tell me a three sentence bedtime story about a unicorn.");
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            var responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.Message, responseItem.Type);
            Assert.IsInstanceOf<Message>(responseItem);
            var messageItem = responseItem as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            var textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}: {messageItem}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_01_02_SimpleTestInput_Streaming()
        {
            Assert.NotNull(OpenAIClient.ResponsesEndpoint);
            var response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync("Tell me a three sentence bedtime story about a unicorn.", async (@event, sseEvent) =>
            {
                Console.WriteLine($"{@event}: {sseEvent.ToJsonString()}");
                await Task.CompletedTask;
            });
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            var responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.Message, responseItem.Type);
            Assert.IsInstanceOf<Message>(responseItem);
            var messageItem = responseItem as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            var textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}: {messageItem}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_02_01_FunctionToolCall()
        {
            Assert.NotNull(OpenAIClient.ResponsesEndpoint);
            var conversation = new List<IResponseItem>
            {
                new Message(Role.System, "You are a helpful weather assistant. Always ask the user for their location."),
                new Message(Role.User, "What's the weather like today?"),
            };
            var tools = new List<Tool>
            {
                Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync)),
            };
            var request = new CreateResponseRequest(conversation, Model.GPT4_1_Nano, tools: tools, toolChoice: "none");
            var response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            var responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.Message, responseItem.Type);
            Assert.IsInstanceOf<Message>(responseItem);
            var messageItem = responseItem as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            var textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}:{textContent.Text}");
            response.PrintUsage();
            conversation.Add(messageItem);
            conversation.Add(new Message(Role.User, "I'm currently in San Francisco"));
            request = new(conversation, Model.GPT4_1_Nano, tools: tools, toolChoice: "auto");
            response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.FunctionCall, responseItem.Type);
            Assert.IsInstanceOf<FunctionToolCall>(responseItem);
            var usedTool = responseItem as FunctionToolCall;
            conversation.Add(usedTool);
            Assert.NotNull(usedTool);
            Assert.IsNotEmpty(usedTool.Name);
            Assert.IsTrue(usedTool.Name.Contains(nameof(WeatherService.GetCurrentWeatherAsync)));
            Assert.NotNull(usedTool.Arguments);
            Console.WriteLine($"{usedTool.Name}: {usedTool.Arguments}");
            response.PrintUsage();
            var functionResult = await usedTool.InvokeFunctionAsync();
            Assert.IsNotNull(functionResult);
            Console.WriteLine($"{usedTool.Name} Result: {functionResult}");
            conversation.Add(functionResult);
            request = new(conversation, Model.GPT4_1_Nano, tools: tools, toolChoice: "none");
            response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.Message, responseItem.Type);
            Assert.IsInstanceOf<Message>(responseItem);
            messageItem = responseItem as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}: {messageItem}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_02_02_FunctionToolCall_Streaming()
        {
            Assert.NotNull(OpenAIClient.ResponsesEndpoint);
            var conversation = new List<IResponseItem>
            {
                new Message(Role.System, "You are a helpful assistant."),
                new Message(Role.User, "What time is it?"),
            };
            var tools = new List<Tool>
            {
                Tool.GetOrCreateTool(typeof(DateTimeUtility), nameof(DateTimeUtility.GetDateTime))
            };
            var request = new CreateResponseRequest(conversation, Model.GPT4_1_Nano, tools: tools);

            async Task StreamCallback(string @event, IServerSentEvent sseEvent)
            {
                switch (sseEvent)
                {
                    case Message messageItem:
                        Assert.NotNull(messageItem);
                        conversation.Add(messageItem);
                        break;
                    case FunctionToolCall functionToolCall:
                        Assert.NotNull(functionToolCall);
                        Assert.IsNotEmpty(functionToolCall.Name);
                        Assert.IsTrue(functionToolCall.Name.Contains(nameof(DateTimeUtility.GetDateTime)));
                        Assert.NotNull(functionToolCall.Arguments);
                        conversation.Add(functionToolCall);
                        var output = await functionToolCall.InvokeFunctionAsync();
                        conversation.Add(output);
                        break;
                }
            }

            var response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request, StreamCallback);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            var responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.FunctionCall, responseItem.Type);
            Assert.IsInstanceOf<FunctionToolCall>(responseItem);
            var usedTool = responseItem as FunctionToolCall;
            Assert.NotNull(usedTool);
            Assert.IsNotEmpty(usedTool.Name);
            Assert.IsTrue(usedTool.Name.Contains(nameof(DateTimeUtility.GetDateTime)));
            Assert.NotNull(usedTool.Arguments);
            Console.WriteLine($"{usedTool.Name}: {usedTool.Arguments}");
            response.PrintUsage();
            response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(new(conversation, Model.GPT4_1_Nano, tools: tools), StreamCallback);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.Message, responseItem.Type);
            Assert.IsInstanceOf<Message>(responseItem);
            var messageItem = responseItem as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            var textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}: {messageItem}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_03_01_Reasoning()
        {
            Assert.NotNull(OpenAIClient.ResponsesEndpoint);
            var request = new CreateResponseRequest("How much wood would a woodchuck chuck?", Model.O3Mini, reasoning: ReasoningEffort.High);
            var response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            var reasoningItem = response.Output.FirstOrDefault(i => i.Type == ResponseItemType.Reasoning) as ReasoningItem;
            Assert.NotNull(reasoningItem);
            Assert.IsNotEmpty(reasoningItem.Summary);
            var responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.Message, responseItem.Type);
            Assert.IsInstanceOf<Message>(responseItem);
            var messageItem = responseItem as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            var textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}: {messageItem}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_03_02_Reasoning_Streaming()
        {
            Message messageItem = null;
            ReasoningItem reasoningItem = null;
            Assert.NotNull(OpenAIClient.ResponsesEndpoint);
            var request = new CreateResponseRequest("How much wood would a woodchuck chuck?", Model.O3Mini, reasoning: ReasoningEffort.High);

            async Task StreamCallback(string @event, IServerSentEvent sseEvent)
            {
                switch (sseEvent)
                {
                    case ReasoningItem ri:
                        Assert.NotNull(ri);
                        reasoningItem = ri;
                        break;
                    case Message m:
                        Assert.NotNull(m);
                        messageItem = m;
                        break;
                }
                await Task.CompletedTask;
            }

            var response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request, StreamCallback);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            Assert.NotNull(reasoningItem);
            Assert.IsNotEmpty(reasoningItem.Summary);
            var lastItem = response.Output.LastOrDefault();
            Assert.NotNull(lastItem);
            Assert.AreEqual(ResponseItemType.Message, lastItem.Type);
            Assert.IsInstanceOf<Message>(lastItem);
            Assert.AreEqual(lastItem.Id, messageItem.Id);
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            var textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}: {messageItem}");
            response.PrintUsage();
        }

        [Test]
        public async Task Test_04_01_JsonSchema()
        {
            Assert.IsNotNull(OpenAIClient.ResponsesEndpoint);

            var messages = new List<IResponseItem>
            {
                new Message(Role.System, "You are a helpful math tutor. Guide the user through the solution step by step."),
                new Message(Role.User, "how can I solve 8x + 7 = -23")
            };

            var request = new CreateResponseRequest(messages, model: Model.GPT4_1_Nano);
            var (mathResponse, response) = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync<MathResponse>(request);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            Assert.NotNull(mathResponse);
            Assert.IsNotEmpty(mathResponse.Steps);

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
            response.PrintUsage();
        }

        [Test]
        public async Task Test_04_02_JsonSchema_Streaming()
        {
            Assert.IsNotNull(OpenAIClient.ResponsesEndpoint);

            var messages = new List<IResponseItem>
            {
                new Message(Role.System, "You are a helpful math tutor. Guide the user through the solution step by step."),
                new Message(Role.User, "how can I solve 8x + 7 = -23")
            };

            Task StreamCallback(string @event, IServerSentEvent sseEvent)
            {
                switch (sseEvent)
                {
                    case Message messageItem:
                        Assert.NotNull(messageItem);
                        var matchSchema = messageItem.FromSchema<MathResponse>();
                        Assert.NotNull(matchSchema);
                        Assert.IsNotEmpty(matchSchema.Steps);

                        for (var i = 0; i < matchSchema.Steps.Count; i++)
                        {
                            var step = matchSchema.Steps[i];
                            Assert.IsNotNull(step.Explanation);
                            Console.WriteLine($"Step {i}: {step.Explanation}");
                            Assert.IsNotNull(step.Output);
                            Console.WriteLine($"Result: {step.Output}");
                        }

                        Assert.IsNotNull(matchSchema.FinalAnswer);
                        Console.WriteLine($"Final Answer: {matchSchema.FinalAnswer}");
                        break;
                }

                return Task.CompletedTask;
            }

            var request = new CreateResponseRequest(messages, model: Model.GPT4_1_Nano);
            var (mathResponse, response) = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync<MathResponse>(request, StreamCallback);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            Assert.NotNull(mathResponse);
            Assert.IsNotEmpty(mathResponse.Steps);
            response.PrintUsage();
        }

        [Test]
        public async Task Test_05_01_Prompts()
        {
            Assert.NotNull(OpenAIClient.ResponsesEndpoint);

            var conversation = new List<IResponseItem>
            {
                new Message(Role.User, "What's the weather like today?"),
            };
            var tools = new List<Tool>
            {
                Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync)),
            };
            var request = new CreateResponseRequest(
                input: conversation,
                model: Model.GPT4_1_Nano,
                prompt: new Prompt("pmpt_685c102c61608193b3654325fa76fc880b22337c811a3a71"),
                tools: tools,
                toolChoice: "none");
            var response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            var responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.Message, responseItem.Type);
            Assert.IsInstanceOf<Message>(responseItem);
            var messageItem = responseItem as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            var textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}:{textContent.Text}");
            response.PrintUsage();
            conversation.Add(messageItem);
            conversation.Add(new Message(Role.User, "I'm currently in San Francisco"));
            request = new(conversation, Model.GPT4_1_Nano, tools: tools, toolChoice: "auto");
            response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.FunctionCall, responseItem.Type);
            Assert.IsInstanceOf<FunctionToolCall>(responseItem);
            var usedTool = responseItem as FunctionToolCall;
            conversation.Add(usedTool);
            Assert.NotNull(usedTool);
            Assert.IsNotEmpty(usedTool.Name);
            Assert.IsTrue(usedTool.Name.Contains(nameof(WeatherService.GetCurrentWeatherAsync)));
            Assert.NotNull(usedTool.Arguments);
            Console.WriteLine($"{usedTool.Name}: {usedTool.Arguments}");
            response.PrintUsage();
            var functionResult = await usedTool.InvokeFunctionAsync();
            Assert.IsNotNull(functionResult);
            Console.WriteLine($"{usedTool.Name} Result: {functionResult}");
            conversation.Add(functionResult);
            request = new(conversation, Model.GPT4_1_Nano, tools: tools, toolChoice: "none");
            response = await OpenAIClient.ResponsesEndpoint.CreateModelResponseAsync(request);
            Assert.NotNull(response);
            Assert.IsNotEmpty(response.Id);
            Assert.AreEqual(ResponseStatus.Completed, response.Status);
            responseItem = response.Output.LastOrDefault();
            Assert.NotNull(responseItem);
            Assert.AreEqual(ResponseItemType.Message, responseItem.Type);
            Assert.IsInstanceOf<Message>(responseItem);
            messageItem = responseItem as Message;
            Assert.NotNull(messageItem);
            Assert.IsNotEmpty(messageItem!.Content);
            Assert.IsInstanceOf<Responses.TextContent>(messageItem.Content[0]);
            textContent = messageItem.Content[0] as Responses.TextContent;
            Assert.NotNull(textContent);
            Assert.IsNotEmpty(textContent!.Text);
            Console.WriteLine($"{messageItem.Role}: {messageItem}");
            response.PrintUsage();
        }
    }
}
