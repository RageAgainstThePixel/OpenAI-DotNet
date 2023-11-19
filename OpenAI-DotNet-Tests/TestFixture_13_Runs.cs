using NUnit.Framework;
using OpenAI.Assistants;
using OpenAI.Models;
using OpenAI.Tests.Weather;
using OpenAI.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal class TestFixture_13_Runs : AbstractTestFixture
    {
        private static CreateThreadRequest TestThreadRequest { get; } = new CreateThreadRequest(
            new List<Message>
            {
                "Test message"
            },
            new Dictionary<string, string>
            {
                ["test"] = "data"
            }
        );

        private static AssistantRequest TestAssistantRequest { get; } = new AssistantRequest("gpt-3.5-turbo-1106");

        [Test]
        public async Task Test_01_CreateRun()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);

            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThreadRequest);
            var request = new CreateRunRequest(assistant, "Run test instructions", Model.GPT3_5_Turbo, null, new Dictionary<string, string>
            {
                ["key"] = "value"
            });
            var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);

            Assert.IsNotNull(run);
            Assert.AreEqual("gpt-3.5-turbo", run.Model);
            Assert.AreEqual("Run test instructions", run.Instructions);

            Assert.IsNotNull(run.Metadata);
            Assert.Contains("key", run.Metadata.Keys.ToList());
            Assert.AreEqual("value", run.Metadata["key"]);
        }

        [Test]
        public async Task Test_02_CreateThreadAndRun()
        {
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
            var request = new CreateThreadAndRunRequest(new CreateThreadRequest(new List<Message> { "thread message text" }), assistant, "Run Test Instructions", Model.GPT3_5_Turbo, null, new Dictionary<string, string> { ["test"] = "data" });
            var run = await OpenAIClient.ThreadsEndpoint.CreateThreadAndRunAsync(request);

            Assert.IsNotNull(run);
            Assert.AreEqual("gpt-3.5-turbo", run.Model);
            Assert.AreEqual("Run test instructions", run.Instructions);

            Assert.IsNotNull(run.Metadata);
            Assert.Contains("key", run.Metadata.Keys.ToList());
            Assert.AreEqual("value", run.Metadata["key"]);

            Assert.IsNotNull(run.ThreadId);
        }

        [Test]
        public async Task Test_03_ListRuns()
        {
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThreadRequest);
            var request = new CreateRunRequest(assistant);
            var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread, request);
            Assert.IsNotNull(run);
            var list = await OpenAIClient.ThreadsEndpoint.ListRunsAsync(thread);
            Assert.IsNotNull(list);
            Assert.IsNotEmpty(list.Items);

            foreach (var threadRun in list.Items)
            {
                var retrievedRun = await OpenAIClient.ThreadsEndpoint.RetrieveRunAsync(thread, threadRun);
                Assert.IsNotNull(retrievedRun);
                Console.WriteLine($"[{retrievedRun.ThreadId}] -> {retrievedRun.Id}");
            }
        }

        [Test]
        public async Task Test_04_ModifyRun()
        {
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
            Assert.IsNotNull(assistant);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThreadRequest);
            Assert.IsNotNull(thread);
            var request = new CreateRunRequest(assistant);
            var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);
            Assert.IsNotNull(run);

            // run in Queued and InProgress can't be modified
            run = await WaitRunPassThroughStatusAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);

            var modified = await OpenAIClient.ThreadsEndpoint.ModifyRunAsync(
                thread.Id,
                run.Id,
                new Dictionary<string, string>
                {
                    ["key"] = "value"
                });

            Assert.IsNotNull(modified);
            Assert.AreEqual(run.Id, modified.Id);
            Assert.IsNotNull(modified.Metadata);
            Assert.Contains("key", modified.Metadata.Keys.ToList());
            Assert.AreEqual("value", modified.Metadata["key"]);
        }

        [Test]
        public async Task Test_05_CancelRun()
        {
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThreadRequest);
            var request = new CreateRunRequest(assistant);
            var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);
            Assert.IsNotNull(run);
            run = await OpenAIClient.ThreadsEndpoint.CancelRunAsync(thread.Id, run.Id);
            Assert.IsNotNull(run);
            // waiting while run in Queued and InProgress
            run = await WaitRunPassThroughStatusAsync(thread.Id, run.Id, RunStatus.Cancelling);

            Assert.AreEqual(RunStatus.Cancelled, run.Status);
        }

        [Test]
        public async Task Test_06_SubmitToolOutput()
        {
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
            var createThreadRequest = new CreateThreadRequest(new List<Message>
            {
                "I'm in Kuala-Lumpur, please tell me what's the temperature in celsius now?"
            });

            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(createThreadRequest);

            var function = new Function(
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
                            ["enum"] = new JsonArray { "celsius", "fahrenheit" }
                        }
                    },
                    ["required"] = new JsonArray { "location", "unit" }
                });

            var request = new CreateRunRequest(assistant, tools: new Tool[] { function });
            var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);

            // waiting while run in Queued and InProgress
            run = await WaitRunPassThroughStatusAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);

            Assert.AreEqual(RunStatus.RequiresAction, run.Status);
            Assert.IsNotNull(run.RequiredAction);
            Assert.AreEqual("submit_tool_outputs", run.RequiredAction.Type);
            Assert.IsNotNull(run.RequiredAction.SubmitToolOutputs);
            Assert.IsNotEmpty(run.RequiredAction.SubmitToolOutputs.ToolCalls);
            Assert.AreEqual(1, run.RequiredAction.SubmitToolOutputs.ToolCalls.Count);
            var toolCall = run.RequiredAction.SubmitToolOutputs.ToolCalls[0];
            Assert.AreEqual("function", toolCall.Type);
            Assert.IsNotNull(toolCall.FunctionCall);
            Assert.AreEqual(nameof(WeatherService.GetCurrentWeather), toolCall.FunctionCall.Name);
            Assert.IsNotNull(toolCall.FunctionCall.Arguments);

            var functionArgs = JsonSerializer.Deserialize<WeatherArgs>(toolCall.FunctionCall.Arguments);
            var functionResult = WeatherService.GetCurrentWeather(functionArgs);
            var submitRequest = new SubmitToolOutputsRequest(new List<ToolOutput>
            {
                new ToolOutput(toolCall.Id, functionResult)
            });

            run = await OpenAIClient.ThreadsEndpoint.SubmitToolOutputsAsync(thread.Id, run.Id, submitRequest);

            // waiting while run in Queued and InProgress
            run = await WaitRunPassThroughStatusAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);
            Assert.AreEqual(RunStatus.Completed, run.Status);
        }

        [Test]
        public async Task Test_07_ListRunSteps()
        {
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThreadRequest);
            var request = new CreateRunRequest(assistant);
            var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);

            // waiting while run in Queued and InProgress
            run = await WaitRunPassThroughStatusAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);

            var list = await OpenAIClient.ThreadsEndpoint.ListRunStepsAsync(thread.Id, run.Id);

            Assert.IsNotNull(list);
            Assert.IsNotEmpty(list.Items);

            foreach (var step in list.Items)
            {
                var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveRunStepAsync(thread.Id, run.Id, step.Id);

                Assert.IsNotNull(retrieved);

                Console.WriteLine($"[{retrieved.ThreadId}] -> {retrieved.Id}");
            }
        }

        private async Task<RunResponse> WaitRunPassThroughStatusAsync(string threadId, string runId, params RunStatus[] statuses)
        {
            var loopCounter = 0;
            RunResponse runResponse;

            do
            {
                if (++loopCounter > 10)
                {
                    Assert.Fail($"Spent too much in long in {string.Join(',', statuses)} statuses");
                }

                await Task.Delay(2000);
                runResponse = await OpenAIClient.ThreadsEndpoint.RetrieveRunAsync(threadId, runId);
            } while (statuses.Contains(runResponse.Status));

            return runResponse;
        }
    }
}