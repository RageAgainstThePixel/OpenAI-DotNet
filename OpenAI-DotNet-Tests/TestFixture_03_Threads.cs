// Licensed under the MIT License. See LICENSE in the project root for license information.

using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Files;
using OpenAI.Models;
using OpenAI.Tests.StructuredOutput;
using OpenAI.Tests.Weather;
using OpenAI.Threads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    /// <summary>
    /// https://github.com/openai/openai-cookbook/blob/main/examples/Assistants_API_overview_python.ipynb
    /// </summary>
    internal class TestFixture_03_Threads : AbstractTestFixture
    {
        [Test]
        public async Task Test_01_Threads()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            ThreadResponse thread = null;

            try
            {
                thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(new(
                    messages: new List<Message>
                    {
                        "Test message"
                    },
                    metadata: new Dictionary<string, string>
                    {
                        ["test"] = nameof(Test_01_Threads)
                    }));
                Assert.IsNotNull(thread);
                Assert.IsNotNull(thread.Metadata);
                Assert.IsNotEmpty(thread.Metadata);
                Console.WriteLine($"Create thread {thread.Id} -> {thread.CreatedAt}");

                var retrievedThread = await thread.UpdateAsync();
                Assert.IsNotNull(retrievedThread);
                Assert.AreEqual(retrievedThread.Id, thread.Id);
                Assert.IsNotNull(retrievedThread.Metadata);
                Assert.IsNotEmpty(retrievedThread.Metadata);
                Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");

                var newMetadata = new Dictionary<string, string>
                {
                    ["test"] = nameof(Test_01_Threads)
                };
                var modifiedThread = await thread.ModifyAsync(newMetadata);
                Assert.IsNotNull(modifiedThread);
                Assert.AreEqual(thread.Id, modifiedThread.Id);
                Assert.IsNotNull(modifiedThread.Metadata);
                Assert.IsNotEmpty(retrievedThread.Metadata);
                Console.WriteLine($"Modify thread {modifiedThread.Id} -> {modifiedThread.Metadata["test"]}");
            }
            finally
            {
                if (thread != null)
                {
                    await thread.DeleteAsync();
                }
            }
        }

        [Test]
        public async Task Test_02_Thread_Messages()
        {
            Assert.IsNotNull(OpenAIClient.ThreadsEndpoint);
            const string testFilePath = "assistant_test_1.txt";
            await File.WriteAllTextAsync(testFilePath, "Knowledge is power!");
            Assert.IsTrue(File.Exists(testFilePath));
            FileResponse file = null;

            try
            {
                try
                {
                    file = await OpenAIClient.FilesEndpoint.UploadFileAsync(testFilePath, FilePurpose.Assistants);
                    Assert.NotNull(file);
                }
                finally
                {
                    if (File.Exists(testFilePath))
                    {
                        File.Delete(testFilePath);
                    }

                    Assert.IsFalse(File.Exists(testFilePath));
                }

                ThreadResponse thread = null;

                try
                {
                    thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync();
                    Assert.NotNull(thread);
                    var message = await thread.CreateMessageAsync("hello world!");
                    Assert.NotNull(message);
                    message = await thread.CreateMessageAsync(new(
                        content: "Test create message",
                        attachments: new[] { new Attachment(file.Id, Tool.FileSearch) },
                        metadata: new Dictionary<string, string>
                        {
                            ["test"] = nameof(Test_02_Thread_Messages)
                        }));
                    Assert.NotNull(message);
                    Assert.NotNull(message.Attachments);
                    Assert.IsNotEmpty(message.Attachments);
                    var message1 = await thread.CreateMessageAsync(new("Test message 1", Role.Assistant));
                    Assert.NotNull(message1);
                    var message2 = await thread.CreateMessageAsync(new("Test message 2"));
                    Assert.NotNull(message2);
                    var list = await thread.ListMessagesAsync();
                    Assert.NotNull(list);
                    Assert.IsNotEmpty(list.Items);

                    foreach (var msg in list.Items)
                    {
                        Assert.NotNull(msg);
                        var retrievedMsg = await thread.RetrieveMessageAsync(msg);
                        Assert.NotNull(retrievedMsg);
                        Console.WriteLine($"[{retrievedMsg.Id}] {retrievedMsg.Role}: {retrievedMsg.PrintContent()}");
                        var updatedMsg = await msg.UpdateAsync();
                        Assert.IsNotNull(updatedMsg);
                    }

                    var guid = Guid.NewGuid().ToString();
                    var metadata = new Dictionary<string, string>
                    {
                        ["test"] = guid
                    };
                    var modified = await message.ModifyAsync(metadata);
                    Assert.IsNotNull(modified);
                    Assert.IsNotNull(modified.Metadata);
                    Assert.IsTrue(modified.Metadata["test"].Equals(guid));
                    Console.WriteLine($"Modify message metadata: {modified.Id} -> {modified.Metadata["test"]}");
                    metadata.Add("test2", Guid.NewGuid().ToString());
                    var modifiedThreadMessage = await thread.ModifyMessageAsync(modified, metadata);
                    Assert.IsNotNull(modifiedThreadMessage);
                    Assert.IsNotNull(modifiedThreadMessage.Metadata);
                    Console.WriteLine($"Modify message metadata: {modifiedThreadMessage.Id} -> {string.Join("\n", modifiedThreadMessage.Metadata.Select(meta => $"[{meta.Key}] {meta.Value}"))}");
                }
                finally
                {
                    if (thread != null)
                    {
                        var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                        Assert.IsTrue(isDeleted);
                    }
                }
            }
            finally
            {
                if (file != null)
                {
                    var isDeleted = await OpenAIClient.FilesEndpoint.DeleteFileAsync(file);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_03_01_CreateRun()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new CreateAssistantRequest(
                    name: "Math Tutor",
                    instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less. Your responses should be formatted in JSON.",
                    model: Model.GPT4o,
#pragma warning disable CS0618 // Type or member is obsolete
                    responseFormat: TextResponseFormat.Json
#pragma warning restore CS0618 // Type or member is obsolete
                ));
            ThreadResponse thread = null;

            try
            {
                Assert.NotNull(assistant);
                thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync();
                Assert.NotNull(thread);
                var message = await thread.CreateMessageAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?");
                Assert.NotNull(message);
                var run = await thread.CreateRunAsync(assistant);
                Assert.IsNotNull(run);
                run = await run.WaitForStatusChangeAsync();
                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
                var messages = await thread.ListMessagesAsync();

                foreach (var response in messages.Items)
                {
                    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
                }

                run = await thread.CreateRunAsync(new CreateRunRequest(assistant));
                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Queued);

                try
                {
                    var runCancelled = await run.CancelAsync();
                    Assert.IsNotNull(runCancelled);
                    Assert.IsTrue(runCancelled);
                }
                catch (Exception e)
                {
                    // Sometimes runs will get stuck in Cancelling state,
                    // or will say it is already cancelled, but it was not,
                    // so for now we just log when it happens.
                    Console.WriteLine(e);

                    if (e is HttpRequestException httpException)
                    {
                        if (!httpException.Message.Contains("Cannot cancel run with status"))
                        {
                            throw;
                        }
                    }
                }

                run = await thread.RetrieveRunAsync(run);
                Assert.IsTrue(run.Status is RunStatus.Cancelled or RunStatus.Cancelling or RunStatus.Completed);
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_03_02_01_CreateRun_Streaming()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new CreateAssistantRequest(
                    name: "Math Tutor",
                    instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
                    model: Model.GPT4o));
            Assert.NotNull(assistant);
            ThreadResponse thread = null;

            try
            {
                thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync();
                Assert.NotNull(thread);
                var message = await thread.CreateMessageAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?");
                Assert.NotNull(message);

                var run = await thread.CreateRunAsync(assistant, async streamEvent =>
                {
                    Console.WriteLine(streamEvent.ToJsonString());

                    switch (streamEvent)
                    {
                        case RunResponse runEvent:
                            Assert.NotNull(runEvent);
                            break;
                        case RunStepResponse runStepEvent:
                            Assert.NotNull(runStepEvent);
                            switch (runStepEvent.Object)
                            {
                                case "thread.run.step.delta":
                                    Assert.NotNull(runStepEvent.Delta);
                                    break;
                                default:
                                    Assert.IsNull(runStepEvent.Delta);
                                    break;
                            }
                            break;
                        case ThreadResponse threadEvent:
                            Assert.NotNull(threadEvent);
                            break;
                        case MessageResponse messageEvent:
                            Assert.NotNull(messageEvent);
                            switch (messageEvent.Object)
                            {
                                case "thread.message.delta":
                                    Assert.NotNull(messageEvent.Delta);
                                    Console.WriteLine($"{messageEvent.Object}: \"{messageEvent.Delta.PrintContent()}\"");
                                    break;
                                default:
                                    Console.WriteLine($"{messageEvent.Object}: \"{messageEvent.PrintContent()}\"");
                                    Assert.IsNull(messageEvent.Delta);
                                    break;
                            }
                            break;
                        case Error errorEvent:
                            Assert.NotNull(errorEvent);
                            break;
                    }

                    await Task.CompletedTask;
                });

                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
                var messages = await thread.ListMessagesAsync();

                foreach (var response in messages.Items.Reverse())
                {
                    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
                }
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_03_02_02_CreateRun_Streaming_ToolCalls()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var tools = new List<Tool>
            {
                Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync))
            };
            Assert.IsTrue(tools.All(tool => tool.Function?.Arguments == null), "Expected all tool function arguments to be null");
            var assistantRequest = new CreateAssistantRequest(
                tools: tools,
                instructions: "You are a helpful weather assistant. Use the appropriate unit based on geographical location.");
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(assistantRequest);
            Assert.NotNull(assistant);
            ThreadResponse thread = null;

            try
            {
                async Task StreamEventHandler(IServerSentEvent streamEvent)
                {
                    try
                    {
                        switch (streamEvent)
                        {
                            case ThreadResponse threadResponse:
                                thread = threadResponse;
                                break;
                            case RunResponse runResponse:
                                if (runResponse.Status == RunStatus.RequiresAction)
                                {
                                    var toolOutputs = await assistant.GetToolOutputsAsync(runResponse);

                                    foreach (var toolOutput in toolOutputs)
                                    {
                                        Console.WriteLine($"Tool Output: {toolOutput}");
                                    }

                                    var toolRun = await runResponse.SubmitToolOutputsAsync(toolOutputs, StreamEventHandler);
                                    Assert.NotNull(toolRun);
                                    Assert.IsTrue(toolRun.Status == RunStatus.Completed, $"Failed to complete submit tool outputs! {toolRun.Status}");
                                }
                                break;
                            case Error errorResponse:
                                throw errorResponse.Exception ?? new Exception(errorResponse.Message);
                            default:
                                Console.WriteLine(streamEvent.ToJsonString());
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                var run = await assistant.CreateThreadAndRunAsync("I'm in Kuala-Lumpur, please tell me what's the temperature now?", StreamEventHandler);
                Assert.NotNull(thread);
                Assert.IsNotNull(run);
                run = await run.WaitForStatusChangeAsync();
                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
                var messages = await thread.ListMessagesAsync();
                Assert.NotNull(messages);
                Assert.IsNotEmpty(messages.Items);

                foreach (var response in messages.Items.Reverse())
                {
                    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
                }

                var guid = Guid.NewGuid().ToString();
                var metadata = new Dictionary<string, string>
                {
                    ["test"] = guid
                };
                var modified = await run.ModifyAsync(metadata);
                Assert.IsNotNull(modified);
                Assert.AreEqual(run.Id, modified.Id);
                Assert.IsNotNull(modified.Metadata);
                Assert.Contains("test", modified.Metadata.Keys.ToList());
                Assert.AreEqual(guid, modified.Metadata["test"]);
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_04_01_CreateThreadAndRun()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new CreateAssistantRequest(
                    name: "Math Tutor",
                    instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
                    model: Model.GPT4o));
            Assert.NotNull(assistant);
            ThreadResponse thread = null;

            try
            {
                var run = await assistant.CreateThreadAndRunAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?");
                Assert.IsNotNull(run);
                thread = await run.GetThreadAsync();
                run = await run.WaitForStatusChangeAsync();
                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
                Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
                Assert.NotNull(thread);
                var messages = await thread.ListMessagesAsync();

                foreach (var response in messages.Items)
                {
                    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
                }
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_04_02_CreateThreadAndRun_Streaming()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new CreateAssistantRequest(
                    name: "Math Tutor",
                    instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
                    model: Model.GPT4o));
            Assert.NotNull(assistant);
            ThreadResponse thread = null;

            try
            {
                var run = await assistant.CreateThreadAndRunAsync("I need to solve the equation `3x + 11 = 14`. Can you help me?",
                    async streamEvent =>
                    {
                        Console.WriteLine(streamEvent.ToJsonString());
                        await Task.CompletedTask;
                    });
                Assert.IsNotNull(run);
                thread = await run.GetThreadAsync();
                run = await run.WaitForStatusChangeAsync();
                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
                Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
                Assert.NotNull(thread);
                var messages = await thread.ListMessagesAsync();

                foreach (var response in messages.Items)
                {
                    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
                }
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_04_03_CreateThreadAndRun_Streaming_ToolCalls()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var tools = new List<Tool>
            {
                Tool.GetOrCreateTool(typeof(DateTimeUtility), nameof(DateTimeUtility.GetDateTime))
            };
            Assert.IsTrue(tools.All(tool => tool.Function?.Arguments == null), "Expected all tool function arguments to be null");
            var assistantRequest = new CreateAssistantRequest(
                instructions: "You are a helpful assistant.",
                tools: tools);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(assistantRequest);
            Assert.IsNotNull(assistant);
            ThreadResponse thread = null;
            // check if any exceptions thrown in stream event handler
            var exceptionThrown = false;
            var hasInvokedCallback = false;

            try
            {
                async Task StreamEventHandler(IServerSentEvent streamEvent)
                {
                    hasInvokedCallback = true;
                    Console.WriteLine($"{streamEvent.ToJsonString()}");

                    try
                    {
                        switch (streamEvent)
                        {
                            case ThreadResponse threadResponse:
                                thread = threadResponse;
                                break;
                            case RunResponse runResponse:
                                if (runResponse.Status == RunStatus.RequiresAction)
                                {
                                    var toolOutputs = await assistant.GetToolOutputsAsync(runResponse);
                                    var toolRun = await runResponse.SubmitToolOutputsAsync(toolOutputs, StreamEventHandler);
                                    Assert.NotNull(toolRun);
                                    Assert.IsTrue(toolRun.Status == RunStatus.Completed, $"Failed to complete submit tool outputs! {toolRun.Status}");
                                }

                                break;
                            case Error errorResponse:
                                throw errorResponse.Exception ?? new Exception(errorResponse.Message);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        exceptionThrown = true;
                    }
                }

                var run = await assistant.CreateThreadAndRunAsync("What is today's date?", StreamEventHandler);
                Assert.IsNotNull(run);
                Assert.IsTrue(hasInvokedCallback);
                Assert.NotNull(thread);
                Assert.IsFalse(exceptionThrown);
                Assert.IsTrue(run.Status == RunStatus.Completed, $"Failed to complete run! {run.Status}");
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_04_04_CreateThreadAndRun_SubmitToolOutput()
        {
            var tools = new List<Tool>
            {
                Tool.CodeInterpreter,
                Tool.GetOrCreateTool(typeof(WeatherService), nameof(WeatherService.GetCurrentWeatherAsync))
            };
            Assert.IsTrue(tools.All(tool => tool.Function?.Arguments == null), "Expected all tool function arguments to be null");
            var assistantRequest = new CreateAssistantRequest(tools: tools, instructions: "You are a helpful weather assistant. Use the appropriate unit based on geographical location.");
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(assistantRequest);
            Assert.IsNotNull(assistant);
            ThreadResponse thread = null;

            try
            {
                var run = await assistant.CreateThreadAndRunAsync("I'm in Kuala-Lumpur, please tell me what's the temperature now?");
                thread = await run.GetThreadAsync();
                Assert.NotNull(thread);
                // waiting while run is Queued and InProgress
                run = await run.WaitForStatusChangeAsync();
                Assert.IsNotNull(run);
                Assert.AreEqual(RunStatus.RequiresAction, run.Status);
                Assert.IsNotNull(run.RequiredAction);
                Assert.IsNotNull(run.RequiredAction.SubmitToolOutputs);
                Assert.IsNotEmpty(run.RequiredAction.SubmitToolOutputs.ToolCalls);

                var runStepList = await run.ListRunStepsAsync();
                Assert.IsNotNull(runStepList);
                Assert.IsNotEmpty(runStepList.Items);

                foreach (var runStep in runStepList.Items)
                {
                    Assert.IsNotNull(runStep);
                    Assert.IsNotNull(runStep.Client);
                    var retrievedRunStep = await runStep.UpdateAsync();
                    Assert.IsNotNull(retrievedRunStep);
                    Console.WriteLine($"[{runStep.Id}] {runStep.Status} {runStep.CreatedAt} -> {runStep.ExpiredAt}");
                    var retrieveStepRunStep = await run.RetrieveRunStepAsync(runStep.Id);
                    Assert.IsNotNull(retrieveStepRunStep);
                }

                var toolCall = run.RequiredAction.SubmitToolOutputs.ToolCalls[0];
                Assert.IsTrue(run.RequiredAction.SubmitToolOutputs.ToolCalls.Count == 1);
                Assert.AreEqual("function", toolCall.Type);
                Assert.IsNotNull(toolCall.FunctionCall);
                Assert.IsTrue(toolCall.FunctionCall.Name.Contains(nameof(WeatherService.GetCurrentWeatherAsync)));
                Assert.IsNotNull(toolCall.FunctionCall.Arguments);
                Console.WriteLine($"tool call arguments: {toolCall.FunctionCall.Arguments}");

                // Invoke all the tool call functions and return the tool outputs.
                var toolOutputs = await assistant.GetToolOutputsAsync(run);

                foreach (var toolOutput in toolOutputs)
                {
                    Console.WriteLine($"tool output: {toolOutput}");
                }

                run = await run.SubmitToolOutputsAsync(toolOutputs);
                // waiting while run in Queued and InProgress
                run = await run.WaitForStatusChangeAsync();
                Assert.AreEqual(RunStatus.Completed, run.Status);
                runStepList = await run.ListRunStepsAsync();

                foreach (var runStep in runStepList.Items)
                {
                    Assert.IsNotNull(runStep);
                    Assert.IsNotNull(runStep.Client);
                    var retrievedRunStep = await runStep.UpdateAsync();
                    Assert.IsNotNull(retrievedRunStep);
                    Console.WriteLine($"[{runStep.Id}] {runStep.Status} {runStep.CreatedAt} -> {(runStep.ExpiredAtUnixTimeSeconds.HasValue ? runStep.ExpiredAt : runStep.CompletedAt)}");
                    if (runStep.StepDetails.ToolCalls == null) { continue; }

                    foreach (var runStepToolCall in runStep.StepDetails.ToolCalls)
                    {
                        Console.WriteLine($"[{runStep.Id}][{runStepToolCall.Type}][{runStepToolCall.Id}] {runStepToolCall.FunctionCall.Name}: {runStepToolCall.FunctionCall.Output}");
                    }
                }

                var messages = await run.ListMessagesAsync();
                Assert.IsNotNull(messages);
                Assert.IsNotEmpty(messages.Items);

                foreach (var message in messages.Items.OrderBy(response => response.CreatedAt))
                {
                    Assert.IsNotNull(message);
                    Assert.IsNotEmpty(message.Content);
                    Console.WriteLine($"{message.Role}: {message.PrintContent()}");
                }
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_05_01_CreateThreadAndRun_StructuredOutputs_Streaming()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync<MathResponse>(
                new CreateAssistantRequest(
                    name: "Math Tutor",
                    instructions: "You are a helpful math tutor. Guide the user through the solution step by step.",
                    model: "gpt-4o-2024-08-06"));
            Assert.NotNull(assistant);
            ThreadResponse thread = null;
            // check if any exceptions thrown in stream event handler
            var exceptionThrown = false;

            try
            {
                async Task StreamEventHandler(IServerSentEvent @event)
                {
                    try
                    {
                        switch (@event)
                        {
                            case MessageResponse message:
                                if (message.Status != MessageStatus.Completed)
                                {
                                    Console.WriteLine(@event.ToJsonString());
                                    break;
                                }

                                var mathResponse = message.FromSchema<MathResponse>();
                                Assert.IsNotNull(mathResponse);
                                Assert.IsNotNull(mathResponse.Steps);
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
                                break;
                            default:
                                Console.WriteLine(@event.ToJsonString());
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        exceptionThrown = true;
                        throw;
                    }

                    await Task.CompletedTask;
                }

                var run = await assistant.CreateThreadAndRunAsync("how can I solve 8x + 7 = -23", StreamEventHandler);
                Assert.IsNotNull(run);
                Assert.IsFalse(exceptionThrown);
                thread = await run.GetThreadAsync();
                run = await run.WaitForStatusChangeAsync();
                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
                Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
                Assert.NotNull(thread);
                var messages = await thread.ListMessagesAsync();

                foreach (var response in messages.Items)
                {
                    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
                }
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_06_01_CreateThreadAndRun_Reasoning()
        {
            const string POLICY = "policy = \"\n# TechStore Policy: Customer Service Guidelines\n\n## 1. Introduction\n\nWelcome to our Customer Service Guidelines for TechStore. This policy aims to provide comprehensive guidance to resolve common customer concerns, covering multiple scenarios, including product damage, incorrect orders, refunds, and replacements. The guidelines include decision trees to help agents make informed decisions and offer a consistent customer experience.\n\n## 2. Product Damage Policy\n\nOur product damage policy covers a range of potential issues, from delivery damage to manufacturing defects. The following decision tree helps guide customer service representatives through different scenarios related to damaged products.\n\n### Decision Tree: Damaged Product Resolution\n\n- **Step 1: Damage Reported**\n\n  - **If there is visible damage** (e.g., cracked screen, broken case):\n    Proceed to **Step 2**.\n\n  - **If there is no visible damage**:\n    Proceed to **Step 3c**.\n\n- **Step 2:** Was the damage reported within 7 days of delivery?\n\n  - **Yes:** Proceed to **Step 3a**.\n  - **No:** Proceed to **Step 3b**.\n\n- **Step 3: Eligibility for Replacement/Refund**\n\n  - **a. Visible Damage Reported Within 7 Days:**\n    - **Option 1:** Full replacement if stock is available.\n    - **Option 2:** Refund (customer's choice).\n    - **Agent Action:** Confirm customer's preferred resolution and provide prepaid shipping label for return (if needed).\n  - **b. Visible Damage Reported After 7 Days but Within 30 Days:**\n    - Assess if the damage could be due to customer mishandling.\n      - **Not Customer Fault (e.g. internal screen issue, phone burst):** Offer repair service or replacement (partial fees may apply based on damage).\n      - **Customer Mishandling (e.g. cracked screen, visible damage):** Offer paid repair or replacement, but customer is not eligible for a refund\n  - **c. No visible damage: Assess damage type**\n    - **If battery issue**: Offer discount for new phone or paid replacement of battery\n    - **If software issue**: Offer troubleshooting tips (see troubleshooting section)\n    - **If phone turns off**: Proceed to **Step 4**.\n\n- **Step 4:** Check Product Purchase Date:\n  - **Less than 1 Year:** Eligible for replacement or return with similar product up to 120% value.\n  - **Less than 1 Month:** Eligible for replacement or return with similar product up to 130% value.\n  - **Between 1 and 2 Years:** Eligible for replacement with similar product up to 110% value.\n  - **More than 2 Years:** No refund or replacement possible.\n\n## 3. Incorrect Order Policy\n\n### Decision Tree: Incorrect Order Handling\n\n- **Step 1: Wrong Product Received**\n\n  - **Customer Receives Wrong Item:**\n    - Ask for photo proof of the item received.\n    - Confirm if the product matches anything available in our inventory (to check if it was a mix-up).\n      - **Match Found:** Proceed to **Step 2**.\n      - **No Match Found:** Escalate to the inventory team to verify any logistical errors.\n\n- **Step 2: Replacement and Retrieval**\n\n  - Confirm that the wrong item is in unused condition and eligible for return.\n  - Offer prepaid shipping label for return of incorrect item.\n  - **Stock Availability for Correct Item:**\n    - **In Stock:** Dispatch correct item immediately.\n    - **Out of Stock:** Offer the following options to the customer:\n      - **Option 1:** Full refund.\n      - **Option 2:** Store credit with 10% bonus as goodwill.\n      - **Option 3:** Waitlist for restock.\n\n## 4. Refund Policy\n\nRefunds may be processed depending on specific scenarios and customer preferences. Use the decision tree below to guide customers.\n\n### Decision Tree: Refund Processing\n\n- **Step 1: Reason for Refund Request**\n\n  - **Product Damaged:** Follow guidelines under **Product Damage Policy**.\n  - **Customer Unsatisfied with Product:**\n    - **Step 2:** Is the request made within 30 days of purchase?\n      - **Yes:** Offer refund (excluding return shipping fee) or store credit.\n      - **No:** Advise customer of the 30-day return limit, suggest troubleshooting, or discuss potential product upgrade.\n  - **Wrong Product Delivered:** See **Incorrect Order Policy** for replacement and refund eligibility.\n\n- **Step 3: Condition of Returned Product**\n\n  - **Unused, Original Packaging:** Full refund processed within 5-7 business days.\n  - **Used, but Fault-Free:** Charge restocking fee (15%) and process the remainder of the refund.\n  - **Damaged Due to Customer Handling:** Notify customer of deduction in refund to cover repair/restocking fee.\n\n## 5. Warranty Claims\n\nAll wearable hardware products come with a limited one-year warranty covering manufacturer defects. Warranty claims are subject to the following decision tree.\n\n### **Decision Tree: Warranty Claim Evaluation**\n\n- **Step 1: Warranty Validity**\n\n  - **Product Purchase Date Verified:**\n    - Within 1 year?\n      - **Yes:** Proceed to **Step 2**.\n      - **No:** Offer paid repair options.\n\n- **Step 2: Nature of Defect**\n\n  - **Manufacturing Defect Confirmed:**\n    - Offer free replacement or repair.\n  - **Wear and Tear or Customer Neglect:**\n    - Inform customer that warranty does not cover general wear and tear.\n    - Offer discounted repair.\n\n## 6. Customer Courtesy Compensation\n\nIn certain cases, customers may be eligible for courtesy compensation in the form of store credit or discounts.\n\n### Decision Tree: Eligibility for Courtesy Compensation\n\n- **Step 1: Assess Severity of Issue**\n  - **Major Inconvenience Due to Our Error:** (e.g., repeated delivery issues, incorrect items sent multiple times)\n    - Offer store credit equivalent to 15% of the product value or a discount coupon for future purchases.\n  - **Minor Issue:** (e.g., delayed delivery due to courier but product received in good condition)\n    - Offer free accessory or a 5% discount code.\n\n## 7. Summary of Escalation Paths\n\nFor cases that cannot be resolved using the above decision trees, escalate to:\n\n- **Tier 2 Support:** Issues requiring deeper technical knowledge (e.g., rare hardware malfunctions, software issues that persist after troubleshooting).\n- **Logistics Team:** Situations involving repeated wrong item deliveries, significant delays, or lost packages.\n- **Customer Success Manager:** High-value customers, chronic dissatisfaction, or VIP escalations requiring a tailored solution.\n\n## 8. Record-Keeping and Follow-Ups\n\n- Ensure all interactions are logged in the CRM system.\n- Follow up on any pending replacements, warranty claims, or courtesy compensation within 48 hours.\n- Provide customers with tracking numbers and regular updates on their cases.\n\n## 9. Final Notes\n\nThese guidelines are intended to ensure our customers are supported in every scenario with fair, efficient, and courteous service. Always strive for the best customer experience, listen attentively, and seek the best solution that aligns with both customer needs and company policy.\n";
            const string REQUEST_SUMMARY = "Phone keeps turning off, it has no visible damage";
            const string ORDER_CONTEXT = "Samsung Galaxy S23 bought on January 28th, 2024.";

            Assert.IsNotNull(OpenAIClient.AssistantsEndpoint);

            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new CreateAssistantRequest(
                    name: "Customer Service Policy",
                    instructions: $"This is our customer service policy:\n{POLICY}\n\nYou will be provided with context on a user order.\nYour task is to reply with what the user is eligible for.\nRespond in one sentence.",
                    model: Model.O3Mini));
            Assert.IsNotNull(assistant);
            ThreadResponse thread = null;

            try
            {
                var run = await assistant.CreateThreadAndRunAsync($"Here is context on the order:\nToday is {DateTime.UtcNow}.\nRequest Summary: {REQUEST_SUMMARY}\nOrder Context: {ORDER_CONTEXT}");
                Assert.IsNotNull(run);
                thread = await run.GetThreadAsync();
                run = await run.WaitForStatusChangeAsync();
                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
                Console.WriteLine($"Created thread and run: {run.ThreadId} -> {run.Id} -> {run.CreatedAt}");
                Assert.NotNull(thread);
                var messages = await thread.ListMessagesAsync();

                foreach (var response in messages.Items)
                {
                    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
                }
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }

        [Test]
        public async Task Test_06_02_CreateRun_Reasoning_Streaming()
        {
            const string POLICY = "policy = \"\n# TechStore Policy: Customer Service Guidelines\n\n## 1. Introduction\n\nWelcome to our Customer Service Guidelines for TechStore. This policy aims to provide comprehensive guidance to resolve common customer concerns, covering multiple scenarios, including product damage, incorrect orders, refunds, and replacements. The guidelines include decision trees to help agents make informed decisions and offer a consistent customer experience.\n\n## 2. Product Damage Policy\n\nOur product damage policy covers a range of potential issues, from delivery damage to manufacturing defects. The following decision tree helps guide customer service representatives through different scenarios related to damaged products.\n\n### Decision Tree: Damaged Product Resolution\n\n- **Step 1: Damage Reported**\n\n  - **If there is visible damage** (e.g., cracked screen, broken case):\n    Proceed to **Step 2**.\n\n  - **If there is no visible damage**:\n    Proceed to **Step 3c**.\n\n- **Step 2:** Was the damage reported within 7 days of delivery?\n\n  - **Yes:** Proceed to **Step 3a**.\n  - **No:** Proceed to **Step 3b**.\n\n- **Step 3: Eligibility for Replacement/Refund**\n\n  - **a. Visible Damage Reported Within 7 Days:**\n    - **Option 1:** Full replacement if stock is available.\n    - **Option 2:** Refund (customer's choice).\n    - **Agent Action:** Confirm customer's preferred resolution and provide prepaid shipping label for return (if needed).\n  - **b. Visible Damage Reported After 7 Days but Within 30 Days:**\n    - Assess if the damage could be due to customer mishandling.\n      - **Not Customer Fault (e.g. internal screen issue, phone burst):** Offer repair service or replacement (partial fees may apply based on damage).\n      - **Customer Mishandling (e.g. cracked screen, visible damage):** Offer paid repair or replacement, but customer is not eligible for a refund\n  - **c. No visible damage: Assess damage type**\n    - **If battery issue**: Offer discount for new phone or paid replacement of battery\n    - **If software issue**: Offer troubleshooting tips (see troubleshooting section)\n    - **If phone turns off**: Proceed to **Step 4**.\n\n- **Step 4:** Check Product Purchase Date:\n  - **Less than 1 Year:** Eligible for replacement or return with similar product up to 120% value.\n  - **Less than 1 Month:** Eligible for replacement or return with similar product up to 130% value.\n  - **Between 1 and 2 Years:** Eligible for replacement with similar product up to 110% value.\n  - **More than 2 Years:** No refund or replacement possible.\n\n## 3. Incorrect Order Policy\n\n### Decision Tree: Incorrect Order Handling\n\n- **Step 1: Wrong Product Received**\n\n  - **Customer Receives Wrong Item:**\n    - Ask for photo proof of the item received.\n    - Confirm if the product matches anything available in our inventory (to check if it was a mix-up).\n      - **Match Found:** Proceed to **Step 2**.\n      - **No Match Found:** Escalate to the inventory team to verify any logistical errors.\n\n- **Step 2: Replacement and Retrieval**\n\n  - Confirm that the wrong item is in unused condition and eligible for return.\n  - Offer prepaid shipping label for return of incorrect item.\n  - **Stock Availability for Correct Item:**\n    - **In Stock:** Dispatch correct item immediately.\n    - **Out of Stock:** Offer the following options to the customer:\n      - **Option 1:** Full refund.\n      - **Option 2:** Store credit with 10% bonus as goodwill.\n      - **Option 3:** Waitlist for restock.\n\n## 4. Refund Policy\n\nRefunds may be processed depending on specific scenarios and customer preferences. Use the decision tree below to guide customers.\n\n### Decision Tree: Refund Processing\n\n- **Step 1: Reason for Refund Request**\n\n  - **Product Damaged:** Follow guidelines under **Product Damage Policy**.\n  - **Customer Unsatisfied with Product:**\n    - **Step 2:** Is the request made within 30 days of purchase?\n      - **Yes:** Offer refund (excluding return shipping fee) or store credit.\n      - **No:** Advise customer of the 30-day return limit, suggest troubleshooting, or discuss potential product upgrade.\n  - **Wrong Product Delivered:** See **Incorrect Order Policy** for replacement and refund eligibility.\n\n- **Step 3: Condition of Returned Product**\n\n  - **Unused, Original Packaging:** Full refund processed within 5-7 business days.\n  - **Used, but Fault-Free:** Charge restocking fee (15%) and process the remainder of the refund.\n  - **Damaged Due to Customer Handling:** Notify customer of deduction in refund to cover repair/restocking fee.\n\n## 5. Warranty Claims\n\nAll wearable hardware products come with a limited one-year warranty covering manufacturer defects. Warranty claims are subject to the following decision tree.\n\n### **Decision Tree: Warranty Claim Evaluation**\n\n- **Step 1: Warranty Validity**\n\n  - **Product Purchase Date Verified:**\n    - Within 1 year?\n      - **Yes:** Proceed to **Step 2**.\n      - **No:** Offer paid repair options.\n\n- **Step 2: Nature of Defect**\n\n  - **Manufacturing Defect Confirmed:**\n    - Offer free replacement or repair.\n  - **Wear and Tear or Customer Neglect:**\n    - Inform customer that warranty does not cover general wear and tear.\n    - Offer discounted repair.\n\n## 6. Customer Courtesy Compensation\n\nIn certain cases, customers may be eligible for courtesy compensation in the form of store credit or discounts.\n\n### Decision Tree: Eligibility for Courtesy Compensation\n\n- **Step 1: Assess Severity of Issue**\n  - **Major Inconvenience Due to Our Error:** (e.g., repeated delivery issues, incorrect items sent multiple times)\n    - Offer store credit equivalent to 15% of the product value or a discount coupon for future purchases.\n  - **Minor Issue:** (e.g., delayed delivery due to courier but product received in good condition)\n    - Offer free accessory or a 5% discount code.\n\n## 7. Summary of Escalation Paths\n\nFor cases that cannot be resolved using the above decision trees, escalate to:\n\n- **Tier 2 Support:** Issues requiring deeper technical knowledge (e.g., rare hardware malfunctions, software issues that persist after troubleshooting).\n- **Logistics Team:** Situations involving repeated wrong item deliveries, significant delays, or lost packages.\n- **Customer Success Manager:** High-value customers, chronic dissatisfaction, or VIP escalations requiring a tailored solution.\n\n## 8. Record-Keeping and Follow-Ups\n\n- Ensure all interactions are logged in the CRM system.\n- Follow up on any pending replacements, warranty claims, or courtesy compensation within 48 hours.\n- Provide customers with tracking numbers and regular updates on their cases.\n\n## 9. Final Notes\n\nThese guidelines are intended to ensure our customers are supported in every scenario with fair, efficient, and courteous service. Always strive for the best customer experience, listen attentively, and seek the best solution that aligns with both customer needs and company policy.\n";
            const string REQUEST_SUMMARY = "Phone keeps turning off, it has no visible damage";
            const string ORDER_CONTEXT = "Samsung Galaxy S23 bought on January 28th, 2024.";

            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new CreateAssistantRequest(
                    name: "Customer Service Policy",
                    instructions: $"This is our customer service policy:\n{POLICY}\n\nYou will be provided with context on a user order.\nYour task is to reply with what the user is eligible for.\nRespond in one sentence.",
                    model: Model.O3Mini));
            Assert.NotNull(assistant);
            ThreadResponse thread = null;

            try
            {
                thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync();
                Assert.NotNull(thread);
                var message = await thread.CreateMessageAsync($"Here is context on the order:\nToday is {DateTime.UtcNow}.\nRequest Summary: {REQUEST_SUMMARY}\nOrder Context: {ORDER_CONTEXT}");
                Assert.NotNull(message);

                var run = await thread.CreateRunAsync(assistant, async streamEvent =>
                {
                    Console.WriteLine(streamEvent.ToJsonString());

                    switch (streamEvent)
                    {
                        case RunResponse runEvent:
                            Assert.NotNull(runEvent);
                            break;
                        case RunStepResponse runStepEvent:
                            Assert.NotNull(runStepEvent);
                            switch (runStepEvent.Object)
                            {
                                case "thread.run.step.delta":
                                    Assert.NotNull(runStepEvent.Delta);
                                    break;
                                default:
                                    Assert.IsNull(runStepEvent.Delta);
                                    break;
                            }
                            break;
                        case ThreadResponse threadEvent:
                            Assert.NotNull(threadEvent);
                            break;
                        case MessageResponse messageEvent:
                            Assert.NotNull(messageEvent);
                            switch (messageEvent.Object)
                            {
                                case "thread.message.delta":
                                    Assert.NotNull(messageEvent.Delta);
                                    Console.WriteLine($"{messageEvent.Object}: \"{messageEvent.Delta.PrintContent()}\"");
                                    break;
                                default:
                                    Console.WriteLine($"{messageEvent.Object}: \"{messageEvent.PrintContent()}\"");
                                    Assert.IsNull(messageEvent.Delta);
                                    break;
                            }
                            break;
                        case Error errorEvent:
                            Assert.NotNull(errorEvent);
                            break;
                    }

                    await Task.CompletedTask;
                });

                Assert.IsNotNull(run);
                Assert.IsTrue(run.Status == RunStatus.Completed);
                var messages = await thread.ListMessagesAsync();

                foreach (var response in messages.Items.Reverse())
                {
                    Console.WriteLine($"{response.Role}: {response.PrintContent()}");
                }
            }
            finally
            {
                await assistant.DeleteAsync(deleteToolResources: thread == null);

                if (thread != null)
                {
                    var isDeleted = await thread.DeleteAsync(deleteToolResources: true);
                    Assert.IsTrue(isDeleted);
                }
            }
        }
    }
}
