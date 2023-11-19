using NUnit.Framework;
using OpenAI.Assistants;
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
    /// <summary>
    /// https://github.com/openai/openai-cookbook/blob/main/examples/Assistants_API_overview_python.ipynb
    /// </summary>
    internal class TestFixture_13_Runs : AbstractTestFixture
    {
        private static string threadId;
        private static string assistantId;
        private static string runId;

        [Test]
        public async Task Test_01_CreateRun()
        {
            Assert.NotNull(OpenAIClient.ThreadsEndpoint);
            var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(
                new AssistantRequest(
                    name: "Math Tutor",
                    instructions: "You are a personal math tutor. Answer questions briefly, in a sentence or less.",
                    model: "gpt-4-1106-preview"));
            Assert.NotNull(assistant);
            assistantId = assistant.Id;
            var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync();
            Assert.NotNull(thread);
            threadId = thread.Id;

            try
            {
                var message = OpenAIClient.ThreadsEndpoint.CreateMessageAsync(thread, "I need to solve the equation `3x + 11 = 14`. Can you help me?");
                Assert.NotNull(message);
                var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread, new CreateRunRequest(assistant));
                Assert.IsNotNull(run);
            }
            finally
            {
                await OpenAIClient.ThreadsEndpoint.DeleteThreadAsync(thread);
            }
        }

        [Test]
        public async Task Test_02_CreateThreadAndRun()
        {
            var request = new CreateThreadAndRunRequest(assistantId);
            var run = await OpenAIClient.ThreadsEndpoint.CreateThreadAndRunAsync(request);
            Assert.IsNotNull(run);
            runId = run.Id;
            Assert.IsFalse(string.IsNullOrWhiteSpace(run.ThreadId));
            threadId = run.ThreadId;
        }

        [Test]
        public async Task Test_04_ModifyRun()
        {
            // run in Queued and InProgress can't be modified
            var run = await WaitOnRunAsync(threadId, runId, RunStatus.Queued, RunStatus.InProgress);

            var modified = await OpenAIClient.ThreadsEndpoint.ModifyRunAsync(run,
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
        public async Task Test_03_ListRuns()
        {
            var request = new CreateRunRequest(assistantId);
            var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(threadId, request);
            Assert.IsNotNull(run);
            var list = await OpenAIClient.ThreadsEndpoint.ListRunsAsync(threadId);
            Assert.IsNotNull(list);
            Assert.IsNotEmpty(list.Items);

            foreach (var threadRun in list.Items)
            {
                var retrievedRun = await OpenAIClient.ThreadsEndpoint.RetrieveRunAsync(threadId, threadRun);
                Assert.IsNotNull(retrievedRun);
            }
        }

        //[Test]
        //public async Task Test_05_CancelRun()
        //{
        //    var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
        //    var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThreadRequest);
        //    var request = new CreateRunRequest(assistant);
        //    var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);
        //    Assert.IsNotNull(run);
        //    run = await OpenAIClient.ThreadsEndpoint.CancelRunAsync(thread.Id, run.Id);
        //    Assert.IsNotNull(run);
        //    // waiting while run in Queued and InProgress
        //    run = await WaitOnRunAsync(thread.Id, run.Id, RunStatus.Cancelling);

        //    Assert.AreEqual(RunStatus.Cancelled, run.Status);
        //}

        //[Test]
        //public async Task Test_06_SubmitToolOutput()
        //{
        //    var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
        //    var createThreadRequest = new CreateThreadRequest(new List<Message>
        //    {
        //        "I'm in Kuala-Lumpur, please tell me what's the temperature in celsius now?"
        //    });

        //    var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(createThreadRequest);

        //    var function = new Function(
        //        nameof(WeatherService.GetCurrentWeather),
        //        "Get the current weather in a given location",
        //        new JsonObject
        //        {
        //            ["type"] = "object",
        //            ["properties"] = new JsonObject
        //            {
        //                ["location"] = new JsonObject
        //                {
        //                    ["type"] = "string",
        //                    ["description"] = "The city and state, e.g. San Francisco, CA"
        //                },
        //                ["unit"] = new JsonObject
        //                {
        //                    ["type"] = "string",
        //                    ["enum"] = new JsonArray { "celsius", "fahrenheit" }
        //                }
        //            },
        //            ["required"] = new JsonArray { "location", "unit" }
        //        });

        //    var request = new CreateRunRequest(assistant, tools: new Tool[] { function });
        //    var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);

        //    // waiting while run in Queued and InProgress
        //    run = await WaitOnRunAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);

        //    Assert.AreEqual(RunStatus.RequiresAction, run.Status);
        //    Assert.IsNotNull(run.RequiredAction);
        //    Assert.AreEqual("submit_tool_outputs", run.RequiredAction.Type);
        //    Assert.IsNotNull(run.RequiredAction.SubmitToolOutputs);
        //    Assert.IsNotEmpty(run.RequiredAction.SubmitToolOutputs.ToolCalls);
        //    Assert.AreEqual(1, run.RequiredAction.SubmitToolOutputs.ToolCalls.Count);
        //    var toolCall = run.RequiredAction.SubmitToolOutputs.ToolCalls[0];
        //    Assert.AreEqual("function", toolCall.Type);
        //    Assert.IsNotNull(toolCall.FunctionCall);
        //    Assert.AreEqual(nameof(WeatherService.GetCurrentWeather), toolCall.FunctionCall.Name);
        //    Assert.IsNotNull(toolCall.FunctionCall.Arguments);

        //    var functionArgs = JsonSerializer.Deserialize<WeatherArgs>(toolCall.FunctionCall.Arguments);
        //    var functionResult = WeatherService.GetCurrentWeather(functionArgs);
        //    var submitRequest = new SubmitToolOutputsRequest(new List<ToolOutput>
        //    {
        //        new ToolOutput(toolCall.Id, functionResult)
        //    });

        //    run = await OpenAIClient.ThreadsEndpoint.SubmitToolOutputsAsync(thread.Id, run.Id, submitRequest);

        //    // waiting while run in Queued and InProgress
        //    run = await WaitOnRunAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);
        //    Assert.AreEqual(RunStatus.Completed, run.Status);
        //}

        //[Test]
        //public async Task Test_07_ListRunSteps()
        //{
        //    var assistant = await OpenAIClient.AssistantsEndpoint.CreateAssistantAsync(TestAssistantRequest);
        //    var thread = await OpenAIClient.ThreadsEndpoint.CreateThreadAsync(TestThreadRequest);
        //    var request = new CreateRunRequest(assistant);
        //    var run = await OpenAIClient.ThreadsEndpoint.CreateRunAsync(thread.Id, request);

        //    // waiting while run in Queued and InProgress
        //    run = await WaitOnRunAsync(thread.Id, run.Id, RunStatus.Queued, RunStatus.InProgress);

        //    var list = await OpenAIClient.ThreadsEndpoint.ListRunStepsAsync(thread.Id, run.Id);

        //    Assert.IsNotNull(list);
        //    Assert.IsNotEmpty(list.Items);

        //    foreach (var step in list.Items)
        //    {
        //        var retrieved = await OpenAIClient.ThreadsEndpoint.RetrieveRunStepAsync(thread.Id, run.Id, step.Id);

        //        Assert.IsNotNull(retrieved);

        //        Console.WriteLine($"[{retrieved.ThreadId}] -> {retrieved.Id}");
        //    }
        //}

        private async Task<RunResponse> WaitOnRunAsync(string thread, string run, params RunStatus[] statuses)
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
                runResponse = await OpenAIClient.ThreadsEndpoint.RetrieveRunAsync(thread, run);
            } while (statuses.Contains(runResponse.Status));

            return runResponse;
        }
    }
}